// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace TwitterBotShared.Model
{
    /// <summary>
    /// Represents result data about a tweet
    /// </summary>
    public class TwitterResult
    {
        /// <summary>
        /// Gets or sets the ID of the tweet
        /// </summary>
        public string TweetId { get; set; }

        /// <summary>
        /// Gets or sets the direct link to the tweet
        /// </summary>
        public string TweetUrl { get; set; }

        /// <summary>
        /// Gets or sets the time at which the tweet was created
        /// </summary>
        public DateTime CreationTime { get; set; }    
    }
}
