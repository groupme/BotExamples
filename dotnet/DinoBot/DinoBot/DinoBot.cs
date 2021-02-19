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

        private static readonly Regex DinoRegex = new Regex("(.*\\D|^)(\\d+) (dino|dinolike|dino-like).*");

        private HashSet<string> _canAddressDino = new HashSet<string>();
        private List<string> _dinoAddressTrigger = new List<string>(new string[] { "hey dinobot" });
        private IBotPoster _botPoster;
        private TraceWriter _log;
        private string _botId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dinobot"/> class
        /// </summary>
        /// <param name="botId">ID of the bot to use to post messages</param>
        /// <param name="botPoster">IBotPoster to use to post bot messages</param>
        public Dinobot(string botId, IBotPoster botPoster) : this(botId, botPoster, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dinobot"/> class
        /// </summary>
        /// <param name="botId">ID of the bot to use to post messages</param>
        /// <param name="botPoster">IBotPoster to use to post bot messages</param>
        /// <param name="log">Logger to use. Optional.</param>
        public Dinobot(string botId, IBotPoster botPoster, TraceWriter log)
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
        /// Gets or sets the message to parse
        /// </summary>
        public MessageItem Message { get; set; }

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
            var bot = new Dinobot(botId, botPoster, log);
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
        public async Task<bool> ParseIncomingRequestAsync(HttpRequestMessage request)
        {
            if (request == null)
            {
                return false;
            }

            string content = await request.Content.ReadAsStringAsync();
            Message = JsonSerializer.DeserializeJson<MessageItem>(content);
            return Message?.Text != null;
        }

        /// <summary>
        /// Processes a message and sends dinos if necessary
        /// </summary>
        /// <returns>True if a bot message was sent, false if message was processed with no action</returns>
        public async Task<bool> ProcessMessageAsync()
        {
            if (Message == null)
            {
                return false;
            }

            _log?.Info("Parsed message for " + Message.GroupId);
            if (string.IsNullOrEmpty(Message.GroupId))
            {
                _log?.Info("Not a group message, ignoring");
                return false;
            }
            else
            {
                UpdateEnvironmentVariables();
                return await HandleIncomingMessageAsync(Message, _botId);
            }
        }

        /// <summary>
        /// Updates the environment variables from Azure
        /// </summary>
        private void UpdateEnvironmentVariables()
        {
            UpdateEnvironmentVariableForContainer(CanAddressDinoKey, _canAddressDino);
            UpdateEnvironmentVariableForContainer(DinoAddressTriggerKey, _dinoAddressTrigger);
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
        /// <param name="message">Message to process</param>
        /// <param name="botId">ID of the bot to use to send messages</param>
        /// <returns>True if a bot message was sent</returns>
        private async Task<bool> HandleIncomingMessageAsync(MessageItem message, string botId)
        {
            // Checks for the type of messages Dinobot will respond to, in decreasing priority order
            // TODO: It would be cool if these were some sort of class-based trigger system you added to a collection rather than this if/else block
            string text = message.Text?.ToLower();
            if (await CheckDinoRequest(text, message, botId))
            {
                _log?.Info("Posted Dino");
                return true;
            }
            else if (await CheckDinoAddressed(text, message, botId))
            {
                _log?.Info("Dino responded");
                return true;
            }
            else if (await CheckRandy(text, botId))
            {
                _log?.Info("Posted Randy");
                return true;
            }
            else if (await CheckDinoQuestion(text, message, botId))
            {
                _log?.Info("Posted DinoQ");
                return true;
            }
            else
            {
                _log?.Info("No dino message");
            }

            return false;
        }

        /// <summary>
        /// Checks if the message contained a number of Dinos to send.
        /// 
        /// For example, a message contains the text "3 dinos" will result in 3 dino emojis sent by the bot.
        /// </summary>
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="message">Message being processed</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        private async Task<bool> CheckDinoRequest(string messageText, MessageItem message, string botId)
        {
            Match match = DinoRegex.Match(messageText);
            if (match.Success && match.Groups.Count == 4)
            {
                _log?.Info("Found dino");
                string num = match.Groups[2].Value;
                int dinos = Math.Min(int.Parse(num), MaxDinos);
                _log?.Info("Dinos:" + dinos.ToString());

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
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="message">Message being processed</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        private async Task<bool> CheckDinoAddressed(string messageText, MessageItem message, string botId)
        {
            if ((_canAddressDino.Count == 0 || _canAddressDino.Contains(message.UserId)) && _dinoAddressTrigger.Any(t => messageText.Contains(t)))
            {
                _log?.Info("Dino addressed");
                Attachment existingReply = message.GetExistingReply();
                HttpStatusCode response = await _botPoster.PostEmojiAsync(DinoPack, DinoEmoji, 1, botId, EmojiPostDelayMs, existingReply?.ReplyId ?? null, existingReply?.BaseReplyId ?? null);
                return response == HttpStatusCode.Accepted;
            }
            return false;
        }

        /// <summary>
        /// Checks if the user requests the Randy emoji
        /// </summary>
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        private async Task<bool> CheckRandy(string messageText, string botId)
        {
            if (messageText.Contains("(randy pooping)"))
            {
                HttpStatusCode response = await _botPoster.PostEmojiAsync(RandyPack, RandyEmoji, 1, botId, EmojiPostDelayMs);
                if (response == HttpStatusCode.Accepted)
                {
                    _log?.Info("Posted Randy:" + response.ToString());
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the user asked a question and uses the most sophisticated AI to determine if Dinobot should respond
        /// </summary>
        /// <param name="messageText">Pre-processed text for the message</param>
        /// <param name="message">The message being processed</param>
        /// <param name="botId">ID of the bot to send the message</param>
        /// <returns>True if a message was sent by the bot, otherwise false</returns>
        private async Task<bool> CheckDinoQuestion(string messageText, MessageItem message, string botId)
        {
            if (messageText.Contains("?"))
            {
                Random rand = new Random();
                double val = rand.NextDouble();
                if (val <= ResponseWeight)
                {
                    _log?.Info("Posting response");
                    Attachment existingReply = message.GetExistingReply();
                    HttpStatusCode response = await _botPoster.PostEmojiAsync(DinoPack, DinoEmoji, 1, botId, EmojiPostDelayMs, message.MessageId, existingReply?.BaseReplyId ?? null);
                    return response == HttpStatusCode.Accepted;
                }
            }
            return false;
        }
    }
}
