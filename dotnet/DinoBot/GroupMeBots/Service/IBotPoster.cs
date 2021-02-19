// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using System.Threading.Tasks;

namespace GroupMeBots.Service
{
    /// <summary>
    /// Interface for posting bot messages to GroupMe
    /// </summary>
    public interface IBotPoster
    {
        /// <summary>
        /// Posts a basic text message from a bit
        /// </summary>
        /// <param name="text">Text to send to the group</param>
        /// <param name="botId">ID of the bot that sends the message</param>
        /// <returns>The status of the outgoing operation to post the message</returns>
        Task<HttpStatusCode> PostAsync(string text, string botId);

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
        Task<HttpStatusCode> PostEmojiAsync(int packId, int emojiId, int numToSend, string botId, int delayMs = 0, string replyToMessageId = null, string baseReplyId = null);
    }
}
