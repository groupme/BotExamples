// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using GroupMeBots.Service;
using GroupMeShared.Model;
using GroupMeShared.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace DinoBot
{
    /// <summary>
    /// Dinobot is a bot that checks a message for certain conditions and responds with GroupMe dinosaur emojis
    /// </summary>
    public class Dinobot
    {
        private static readonly Regex DinoRegex = new Regex("(.*\\D|^)(\\d+) (dino|dinolike|dino-like).*");

        private const string BotPostUrl = "https://api.groupme.com/v3/bots/post";
        private const string CanAddressDinoKey = "CanAddressDino";
        private const string DinoAddressTriggerKey = "DinoAddressTrigger";
        private const double ResponseWeight = 0.075;
        private const int MaxDinos = 100;
        private const int DinoPack = 1;
        private const int DinoEmoji = 62;
        private const int RandyPack = 8;
        private const int RandyEmoji = 47;
        private const int EmojiPostDelayMs = 500;

        private HashSet<string> CanAddressDino = new HashSet<string>();
        private List<string> DinoAddressTrigger = new List<string>(new string[] { "hey dinobot" });
        private IBotPoster _botPoster;
        private TraceWriter _log;
        private string _botId;
        private MessageItem _message;

        internal Dinobot(TraceWriter log, string botId, IBotPoster botPoster)
        {
            if (botPoster == null)
            {
                throw new ArgumentNullException(nameof(botPoster));
            }
            if (string.IsNullOrWhiteSpace(botId))
            {
                throw new ArgumentNullException(botId);
            }

            _log = log;
            _botId = botId;
            _botPoster = botPoster;
        }

        /// <summary>
        /// Message called when the Azure function is invoked
        /// 
        /// GroupMe bots can react to messages that are posted to groups.
        /// The message should be included in the content payload, and the botId/token is included in the HTTP request.
        /// </summary>
        /// <param name="req">Incoming HTTP request</param>
        /// <param name="log">Azure logger</param>
        /// <param name="botId">ID of the bot that was invoked</param>
        /// <returns>HTTP response: 
        /// Created (201) if the bot successfully posted a response, 
        /// Success (200) if the message was successfully parsed but no response necessary, 
        /// BadRequest (400) if no message in the payload.
        /// </returns>
        [FunctionName("DinoBot")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(Route = "DinoCallback/{botId}")] HttpRequestMessage req, TraceWriter log, string botId)
        {
            log.Info("Got message callback");
            if (string.IsNullOrWhiteSpace(botId))
            {
                log.Error("No botId");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("No BotId present")
                };
            }

            var botPoster = new BotPoster(BotPostUrl);
            var bot = new Dinobot(log, botId, botPoster);
            bool parsedMessage = await bot.ParseIncomingRequestAsync(req);
            if (parsedMessage)
            {
                bool postedMessage = await bot.ProcessMessageAsync();
                return new HttpResponseMessage(postedMessage ? HttpStatusCode.Created : HttpStatusCode.OK);
            }

            log.Info("No message found in payload");
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("No message in payload")
            };
        }

        /// <summary>
        /// Parses an incoming HTTP request into a GroupMe message
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <returns>True if a message was properly parsed from the request</returns>
        internal async Task<bool> ParseIncomingRequestAsync(HttpRequestMessage request)
        {
            if (request == null)
            {
                return false;
            }

            string content = await request.Content.ReadAsStringAsync();
            _message = JsonSerializer.DeserializeJson<MessageItem>(content);
            return _message?.Text != null;
        }

        /// <summary>
        /// Processes a message and sends dinos if necessary
        /// </summary>
        /// <returns>True if a bot message was sent, false if message was processed with no action</returns>
        internal async Task<bool> ProcessMessageAsync()
        {
            if (_message == null)
            {
                return false;
            }

            _log?.Info("Parsed message for " + _message.GroupId);
            if (string.IsNullOrEmpty(_message.GroupId))
            {
                _log?.Info("Not a group message, ignoring");
                return false;
            }
            else
            {
                UpdateEnvironmentVariables();
                return await HandleIncomingMessageAsync(_log, _message, _botId);
            }
        }

        /// <summary>
        /// Updates the environment variables from Azure
        /// </summary>
        private void UpdateEnvironmentVariables()
        {
            UpdateEnvironmentVariableForContainer(CanAddressDinoKey, CanAddressDino);
            UpdateEnvironmentVariableForContainer(DinoAddressTriggerKey, DinoAddressTrigger);
        }

        /// <summary>
        /// Updates an environment variable that contains a comma-delimited list of items to store in a container
        /// </summary>
        /// <param name="key">Key for the environment variable</param>
        /// <param name="container">Variable in which to store the list</param>
        private void UpdateEnvironmentVariableForContainer(string key, ICollection<string> container)
        {
            string value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(value))
            {
                _log?.Info($"Using {key}: {value}");
                container.Clear();
                foreach (string phrase in value.Split(','))
                {
                    container.Add(phrase);
                }
            }
            else
            {
                _log?.Info($"Using default {key}");
            }
        }

        /// <summary>
        /// Handles an incoming message and sends a bot message if appropriate
        /// </summary>
        /// <param name="log">Logger for the operation</param>
        /// <param name="message">Message to process</param>
        /// <param name="botId">ID of the bot to use to send messages</param>
        /// <returns>True if a bot message was sent</returns>
        private async Task<bool> HandleIncomingMessageAsync(TraceWriter log, MessageItem message, string botId)
        {
            // Checks for the type of messages Dinobot will respond to, in decreasing priority order
            // TODO: It would be cool if these were some sort of class-based trigger system you added to a collection rather than this if/else block
            string text = message.Text?.ToLower();
            if (await CheckDinoRequest(log, text, message, botId))
            {
                log?.Info("Posted Dino");
                return true;
            }
            else if (await CheckDinoAddressed(log, text, message, botId))
            {
                log?.Info("Dino responded");
                return true;
            }
            else if (await CheckRandy(log, text, botId))
            {
                log?.Info("Posted Randy");
                return true;
            }
            else if (await CheckDinoQuestion(log, text, message, botId))
            {
                log?.Info("Posted DinoQ");
                return true;
            }
            else
            {
                log?.Info("No dino message");
            }

            return false;
        }

        /// <summary>
        /// Checks if the message contained a number of Dinos to send.
        /// 
        /// For example, a message contains the text "3 dinos" will result in 3 dino emojis sent by the bot.
        /// </summary>
        /// <param name="log">Logger for the operation</param>
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="message">Message being processed</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        private async Task<bool> CheckDinoRequest(TraceWriter log, string messageText, MessageItem message, string botId)
        {
            Match match = DinoRegex.Match(messageText);
            if (match.Success && match.Groups.Count == 4)
            {
                log?.Info("Found dino");
                string num = match.Groups[2].Value;
                int dinos = Math.Min(int.Parse(num), MaxDinos);
                log?.Info("Dinos:" + dinos.ToString());

                if (dinos > 0)
                {
                    Attachment existingReply = message.GetExistingReply();
                    HttpStatusCode response = await _botPoster.PostEmojiAsync(DinoPack, DinoEmoji, dinos, botId, EmojiPostDelayMs, existingReply?.ReplyId ?? null, existingReply?.BaseReplyId ?? null);
                    return response == HttpStatusCode.Accepted;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the message to see if the user specifically addressed Dinobot.
        /// 
        /// For example, a message containing the test "Hey DinoBot" will result in a dinosaur emoji being sent.
        /// The trigger text and users who can use this trigger are configurable via AppSettings
        /// </summary>
        /// <param name="log">Logger for the operation</param>
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="message">Message being processed</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        private async Task<bool> CheckDinoAddressed(TraceWriter log, string messageText, MessageItem message, string botId)
        {
            if ((CanAddressDino.Count == 0 || CanAddressDino.Contains(message.UserId)) && DinoAddressTrigger.Any(t => messageText.Contains(t)))
            {
                log?.Info("Dino addressed");
                Attachment existingReply = message.GetExistingReply();
                HttpStatusCode response = await _botPoster.PostEmojiAsync(DinoPack, DinoEmoji, 1, botId, EmojiPostDelayMs, existingReply?.ReplyId ?? null, existingReply?.BaseReplyId ?? null);
                return response == HttpStatusCode.Accepted;
            }
            return false;
        }
        /// <summary>
        /// Checks if the user requests the Randy emoji
        /// </summary>
        /// <param name="log">Logger for the operation</param>
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        private async Task<bool> CheckRandy(TraceWriter log, string messageText, string botId)
        {
            if (messageText.Contains("(randy pooping)"))
            {
                HttpStatusCode response = await _botPoster.PostEmojiAsync(RandyPack, RandyEmoji, 1, botId, EmojiPostDelayMs);
                if (response == HttpStatusCode.Accepted)
                {
                    log?.Info("Posted Randy:" + response.ToString());
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the user asked a question and uses the most sophisticated AI to determine if Dinobot should respond
        /// </summary>
        /// <param name="log">Logger for the operation</param>
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        /// <returns></returns>
        private async Task<bool> CheckDinoQuestion(TraceWriter log, string messageText, MessageItem message, string botId)
        {
            if (messageText.Contains("?"))
            {
                Random rand = new Random();
                double val = rand.NextDouble();
                if (val <= ResponseWeight)
                {
                    log?.Info("Posting response");
                    Attachment existingReply = message.GetExistingReply();
                    HttpStatusCode response = await _botPoster.PostEmojiAsync(DinoPack, DinoEmoji, 1, botId, EmojiPostDelayMs, message.MessageId, existingReply?.BaseReplyId ?? null);
                    return response == HttpStatusCode.Accepted;
                }
            }
            return false;
        }
    }
}
