// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace TwitterBotShared.Model
{
    /// <summary>
    /// Twitter authentication result data
    /// </summary>
    [DataContract]
    public class TwitterAuth
    {
        /// <summary>
        /// Gets or sets the type of token
        /// </summary>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
    }
}