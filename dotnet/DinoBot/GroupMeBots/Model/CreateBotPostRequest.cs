// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Serialization;

using GroupMeShared.Model;

namespace GroupMeBots.Model
{
    /// <summary>
    /// Represents a bot message to be posted to GroupMe. Messages in the following JSON format:
    /// 
    /// {
    ///    "bot_id": [botId],
    ///    "text": [messageText],
    ///    "attachments": [arrayOfMessageAttachments]
    /// }
    /// 
    /// for example
    /// 
    /// {
    ///   "bot_id": "123",
    ///   "text": "Dinos are awesome!"
    ///   "attachments": [
    ///     {
    ///       "type": emoji,
    ///       "charmap": [[1,62],[1,62]]
    ///     }
    ///   ]
    /// </summary>
    [DataContract]
    public class CreateBotPostRequest
    {
        /// <summary>
        /// Gets or sets the ID of the bot that is sending the message
        /// </summary>
        [DataMember(Name = "bot_id")]
        public string BotId { get; set; }

        /// <summary>
        /// Gets or sets the text of the message
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the attachments for the message
        /// </summary>
        [DataMember(Name = "attachments")]
        public Attachment[] Attachments { get; set; }
    }
}
