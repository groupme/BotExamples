// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace TwitterBotShared.Model
{
    /// <summary>
    /// Represents the result of a twitter search
    /// </summary>
    [DataContract]
    public class TwitterResponse
    {
        /// <summary>
        /// Gets or sets the list of tweets that were retrieved for a search
        /// </summary>
        [DataMember(Name = "statuses")]
        public TwitterStatus[] Statuses { get; set; }

        /// <summary>
        /// Represents details about a tweet
        /// </summary>
        [DataContract]
        public class TwitterStatus
        {
            /// <summary>
            /// Gets or sets the string form of the tweet ID
            /// </summary>
            [DataMember(Name = "id_str")]
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the text for the tweet
            /// </summary>
            [DataMember(Name = "text")]
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets details about the user that posted the tweet
            /// </summary>
            [DataMember(Name = "user")]
            public TwitterUser User { get; set; }

            /// <summary>
            /// Gets or sets the time at which the tweet was created. 
            /// </summary>
            [DataMember(Name = "created_at")]
            public string CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the number of times the tweet has been retweeted
            /// </summary>
            [DataMember(Name = "retweet_count")]
            public int RetweetCount { get; set; }

            /// <summary>
            /// Gets or sets the retweeted status
            /// </summary>
            [DataMember(Name = "retweeted_status")]
            public TwitterStatus Retweet { get; set; }

            /// <summary>
            /// Gets the UTC time at which the tweet was created
            /// </summary>
            [IgnoreDataMember]
            public DateTime CreatedAtTime
            {
                get
                {
                    DateTime offset;
                    if (!string.IsNullOrEmpty(CreatedAt) && DateTime.TryParseExact(CreatedAt, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out offset))
                    {
                        return offset.ToUniversalTime();
                    }
                    return DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Represents details about a Twitter user
        /// </summary>
        [DataContract]
        public class TwitterUser
        {
            /// <summary>
            /// Gets or sets the Twitter user's full name
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the user's Twitter handle
            /// </summary>
            [DataMember(Name = "screen_name")]
            public string ScreenName { get; set; }
        }
    }
}