// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using GroupMeBots;
using GroupMeShared.Model;
using GroupMeShared.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace DinoBot
{
    /// <summary>
    /// Dinobot is a bot that checks a message for certain conditions and responds with GroupMe dinosaur emojis
    /// </summary>
    public static class Dinobot
    {
        private static readonly Regex DinoRegex = new Regex("(.*\\D|^)(\\d+) (dino|dinolike|dino-like).*");

        private static HashSet<string> CanAddressDino = new HashSet<string>();
        private static List<string> DinoAddressTrigger = new List<string>(new string[] { "hey dinobot" });

        private const string CanAddressDinoKey = "CanAddressDino";
        private const string DinoAddressTriggerKey = "DinoAddressTrigger";
        private const int MaxDinos = 100;
        private const int DinoPack = 1;
        private const int DinoEmoji = 62;
        private const int RandyPack = 8;
        private const int RandyEmoji = 47;
        private const int EmojiPostDelayMs = 500;

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
            if (string.IsNullOrEmpty(botId))
            {
                log.Error("No botId");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            string content = await req.Content.ReadAsStringAsync();
            var message = JsonSerializer.DeserializeJson<MessageItem>(content);
            if (!string.IsNullOrEmpty(message?.Text))
            {
                log.Info("Parsed message for " + message.GroupId);
                if (string.IsNullOrEmpty(message.GroupId))
                {
                    log.Info("Not a group message, ignoring");
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    UpdateEnvironmentVariables(log);
                    bool posted = await HandleIncomingMessage(log, message, botId);
                    return new HttpResponseMessage(posted ? HttpStatusCode.Created : HttpStatusCode.OK);
                }
            }

            log.Info("No message found in payload");
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        private static void UpdateEnvironmentVariables(TraceWriter log)
        {
            string canAddress = Environment.GetEnvironmentVariable(CanAddressDinoKey, EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(canAddress))
            {
                log.Info("Using CanAddress: " + canAddress);
                CanAddressDino.Clear();
                foreach (string user in canAddress.Split(','))
                {
                    CanAddressDino.Add(user);
                }
            }
            else
            {
                log.Info("Using default CanAddress");
            }

            string addressTrigger = Environment.GetEnvironmentVariable(DinoAddressTriggerKey, EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(addressTrigger))
            {
                log.Info("Using AddressTrigger: " + addressTrigger);
                DinoAddressTrigger.Clear();
                foreach (string phrase in addressTrigger.Split(','))
                {
                    DinoAddressTrigger.Add(phrase);
                }
            }
            else
            {
                log.Info("Using default AddressTrigger");
            }
        }

        private static async Task<bool> HandleIncomingMessage(TraceWriter log, MessageItem message, string botId)
        {
            string text = message.Text?.ToLower();
            if (await CheckDinoRequest(log, text, message, botId))
            {
                log.Info("Posted Dino");
                return true;
            }
            else if (await CheckDinoAddressed(log, text, message, botId))
            {
                log.Info("Dino responded");
                return true;
            }
            else if (await CheckRandy(log, text, botId))
            {
                log.Info("Posted Randy");
                return true;
            }
            else if (await CheckDinoQuestion(log, text, message, botId))
            {
                log.Info("Posted DinoQ");
                return true;
            }
            else
            {
                log.Info("No dino message");
            }

            return false;
        }

        private static async Task<bool> CheckDinoRequest(TraceWriter log, string messageText, MessageItem message, string botId)
        {            
            Match match = DinoRegex.Match(messageText);
            if (match.Success && match.Groups.Count == 4)
            {
                log.Info("Found dino");
                string num = match.Groups[2].Value;
                int dinos = Math.Min(int.Parse(num), MaxDinos);
                log.Info("Dinos:" + dinos.ToString());

                if (dinos > 0)
                {
                    Attachment existingReply = message.GetExistingReply();
                    HttpStatusCode response = await BotPoster.PostEmojiAsync(DinoPack, DinoEmoji, dinos, botId, EmojiPostDelayMs, existingReply?.ReplyId ?? null, existingReply?.BaseReplyId ?? null);
                    return response == HttpStatusCode.Accepted;
                }
            }
            return false;
        }

        private static async Task<bool> CheckDinoAddressed(TraceWriter log, string messageText, MessageItem message, string botId)
        {
            if ((CanAddressDino.Count == 0 || CanAddressDino.Contains(message.UserId)) && DinoAddressTrigger.Any(t => messageText.Contains(t)))
            {
                log.Info("Dino addressed");
                Attachment existingReply = message.GetExistingReply();
                HttpStatusCode response = await BotPoster.PostEmojiAsync(DinoPack, DinoEmoji, 1, botId, EmojiPostDelayMs, existingReply?.ReplyId ?? null, existingReply?.BaseReplyId ?? null);
                return response == HttpStatusCode.Accepted;
            }
            return false;
        }

        private static async Task<bool> CheckRandy(TraceWriter log, string messageText, string botId)
        {
            if (messageText.Contains("(randy pooping)"))
            {
                HttpStatusCode response = await BotPoster.PostEmojiAsync(RandyPack, RandyEmoji, 1, botId, EmojiPostDelayMs);
                if (response == HttpStatusCode.Accepted)
                {
                    log.Info("Posted Randy:" + response.ToString());
                    return true;
                }
            }
            return false;
        }

        private static async Task<bool> CheckDinoQuestion(TraceWriter log, string messageText, MessageItem message, string botId)
        {
            if (messageText.Contains("?"))
            {
                Random rand = new Random();
                double val = rand.NextDouble();
                if (val <= 0.075)
                {
                    Attachment existingReply = message.GetExistingReply();
                    HttpStatusCode response = await BotPoster.PostEmojiAsync(DinoPack, DinoEmoji, 1, botId, EmojiPostDelayMs, message.MessageId, existingReply?.BaseReplyId ?? null);
                    return response == HttpStatusCode.Accepted;
                }
            }
            return false;
        }
    }
}
