// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace GroupMeShared.Model
{
    /// <summary>
    /// Represents response metadata
    /// </summary>
    [DataContract]
    public class Meta
    {
        /// <summary>
        /// Gets or sets response metadata code
        /// </summary>
        [DataMember(Name = "code")]
        public int Code { get; set; }
    }
}