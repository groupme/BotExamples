// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using GroupMeShared.Model;
using System.Runtime.Serialization;

namespace GroupMeBot.Model
{
    /// <summary>
    /// Represents the response from a request to create a new bot
    /// </summary>
    [DataContract]
    public class CreateBotResponse
    {
        /// <summary>
        /// Gets or sets the call response metadata
        /// </summary>
        [DataMember(Name = "meta")]
        public Meta ResponseMeta { get; set; }

        /// <summary>
        /// Gets or sets the response details
        /// </summary>
        [DataMember(Name = "response")]
        public BotResponse Response { get; set; }

        /// <summary>
        /// Represents information about a newly created bot
        /// </summary>
        [DataContract]
        public class BotResponse
        {
            /// <summary>
            /// Gets or sets the new bot
            /// </summary>
            [DataMember(Name = "bot")]
            public BotData Bot { get; set; }
        }

        /// <summary>
        /// Represents details about a newly created bot
        /// </summary>
        [DataContract]
        public class BotData
        {
            /// <summary>
            /// Gets or sets the name of the bot
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the newly created bot's ID
            /// </summary>
            [DataMember(Name = "bot_id")]
            public string BotId { get; set; }
        }

    }
}