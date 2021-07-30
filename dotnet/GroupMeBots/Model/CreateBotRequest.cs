// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace GroupMeBot.Model
{
    /// <summary>
    /// Request object for creating a new GroupMe bot
    /// </summary>
    [DataContract]
    public class CreateBotRequest
    {
        /// <summary>
        /// Gets or sets the bot data
        /// </summary>
        [DataMember(Name = "bot")]
        public BotData Bot { get; set; }

        /// <summary>
        /// Data about the bot to create
        /// </summary>
        [DataContract]
        public class BotData
        {
            /// <summary>
            /// Gets or sets the name of the bot. Displayed on all messages the bot posts.
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the ID of the group in which to create the bot
            /// </summary>
            [DataMember(Name = "group_id")]
            public string GroupId { get; set; }

            /// <summary>
            /// Gets or sets the URL that should be called every time a message is posted to the group
            /// </summary>
            [DataMember(Name = "callback_url", EmitDefaultValue = false)]
            public string CallbackUrl { get; set; }

            /// <summary>
            /// Gets or sets the URL of the bot's avatar. Dispalyed on every message posted by the bot.
            /// </summary>
            [DataMember(Name = "avatar_url", EmitDefaultValue = false)]
            public string AvatarUrl { get; set; }
        }
    }
}