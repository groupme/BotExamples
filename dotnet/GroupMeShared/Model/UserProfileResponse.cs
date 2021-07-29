// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace GroupMeShared.Model
{
    /// <summary>
    /// Contains the response for a user's profile
    /// </summary>
    [DataContract]
    public class UserEnvelope
    {
        [DataMember(Name = "response")]
        public User Response { get; set; }
    }
}
