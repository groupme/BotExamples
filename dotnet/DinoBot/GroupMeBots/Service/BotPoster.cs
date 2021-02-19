// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using GroupMeBots.Model;
using GroupMeShared.Model;
using GroupMeShared.Utilities;

namespace GroupMeBots.Service
{
    /// <summary>
    /// <see cref="BotPoster"/> is a class used to post messages to GroupMe with a bot
    /// </summary>
    public class BotPoster : IBotPoster
    {
        private const string EmojiPlaceholder = "�";
        private string _botPostUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotPoster"/> class
        /// </summary>
        /// <param name="botPostUrl">GroupMe server URL to use to post the message</param>
        public BotPoster(string botPostUrl)
        {
            _botPostUrl = botPostUrl;
        }

        /// <summary>
        /// Posts a basic text message from a bit
        /// </summary>
        /// <param name="text">Text to send to the group</param>
        /// <param name="botId">ID of the bot that sends the message</param>
        /// <returns>The status of the outgoing operation to post the message</returns>
        public async Task<HttpStatusCode> PostAsync(string text, string botId)
        {
            var post = new CreateBotPostRequest
            {
                BotId = botId,
                Text = text
            };
            return await PostBotMessage(post);
        }

        /// <summary>
        /// Posts GroupMe emoji to a group from a bot
        /// </summary>
        /// <param name="packId">ID of the emoji pack containing the emoji to send</param>
        /// <param name="emojiId">ID of the emoji within the pack to send</param>
        /// <param name="numToSend">Number of emoji to send with the message</param>
        /// <param name="botId">ID of the bot sending the message</param>
        /// <param name="shouldDelay">(optional) The amount of time, in milliseconds, to delay before posting the message. Useful for replies, to ensure ordering.</param>
        /// <param name="replyToMessageId">(optional) ID of the message to which this message is a reply. Defaults to null.</param>
        /// <param name="baseReplyId">(optional) ID of the base message to which this message is a part of a thread. Defaults to null.</param>
        /// <returns>Status of the outgoing operation to post the message</returns>
        public async Task<HttpStatusCode> PostEmojiAsync(int packId, int emojiId, int numToSend, string botId, int delayMs = 0, string replyToMessageId = null, string baseReplyId = null)
        {
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            // Construct the emoji charmap to attach to the message
            // Emoji charmap is a two dimensional array, in format:
            // [[packId_1,emojiId_1],[packId_2,emojiId_2]...[packId_n,emojiId_n]]
            // Each emoji placeholder in the message will be replaced in order with the appropriate emojiId as defined in the charmap.
            // For these messages, its easy - give text [numToSend] sequential emoji placeholders, and repeat the same emoji [numToSend] times in the charmap
            string text = "";
            int[][] charmap = new int[numToSend][];
            for (int emojiIndex = 0; emojiIndex < numToSend; ++emojiIndex)
            {
                text += EmojiPlaceholder;
                charmap[emojiIndex] = new int[2];
                charmap[emojiIndex][0] = packId;
                charmap[emojiIndex][1] = emojiId;
            }

            // Construct the message attachments
            Attachment[] attachments = new Attachment[string.IsNullOrEmpty(replyToMessageId) ? 1 : 2];
            attachments[0] = new Attachment
            {
                Type = Attachment.Emoji,
                Placeholder = EmojiPlaceholder,
                Charmap = charmap
            };
            if (!string.IsNullOrEmpty(replyToMessageId))
            {
                attachments[1] = new Attachment
                {
                    Type = Attachment.ReplyType,
                    ReplyId = replyToMessageId,
                    BaseReplyId = string.IsNullOrEmpty(baseReplyId) ? replyToMessageId : baseReplyId
                };
            }

            // Construct the message
            var post = new CreateBotPostRequest
            {
                BotId = botId,
                Text = text,
                Attachments = attachments
            };

            // Send the message
            return await PostBotMessage(post);
        }

        /// <summary>
        /// Posts a bot message to the service
        /// </summary>
        /// <param name="request">Request to post</param>
        /// <returns>Response code from the GroupMe service</returns>
        private async Task<HttpStatusCode> PostBotMessage(CreateBotPostRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (StringContent content = JsonSerializer.SerializeToJson(request))
            {
                var client = new HttpClient();
                HttpResponseMessage result = await client.PostAsync(_botPostUrl, content);
                return result != null ? result.StatusCode : HttpStatusCode.ServiceUnavailable;
            }
        }
    }
}