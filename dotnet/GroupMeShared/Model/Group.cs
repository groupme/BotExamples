// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace GroupMeShared.Model
{
    [DataContract]
    public class Group
    {
        /// <summary>
        /// Gets or sets the ID of this chat (should match GroupId)
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique ID for this group
        /// </summary>
        [DataMember(Name = "group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the display name of this group
        /// </summary>
        [DataMember(Name = "name")]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the type of group
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the telephone number used to communicate with this group via SMS
        /// </summary>
        [DataMember(Name = "phone_number")]
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the time (since epoch) since the group was updated
        /// </summary>
        [DataMember(Name = "updated_at")]
        public long UpdatedFromEpoch { get; set; }

        /// <summary>
        /// Gets or sets the time (since epoch) since the group was created
        /// </summary>
        [DataMember(Name = "created_at")]
        public long CreatedAtEpoch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not group is in office mode
        /// </summary>
        [DataMember(Name = "office_mode")]
        public bool OfficeMode { get; set; }

        /// <summary>
        /// Gets or sets the share URL for this group
        /// </summary>
        [DataMember(Name = "share_url")]
        public string ShareUrl { get; set; }

        /// <summary>
        /// Gets or sets the maximum numberof members in a group
        /// </summary>
        [DataMember(Name = "max_members")]
        public int MaxMembers { get; set; }

        /// <summary>
        /// Gets or sets the URL to the avatar for this group 
        /// </summary>
        [DataMember(Name = "image_url")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the creator user identifier.
        /// </summary>
        /// <value>
        /// The creator user identifier.
        /// </value>
        [DataMember(Name = "creator_user_id")]
        public string CreatorUserId { get; set; }
    }

    /// <summary>
    /// Represents a response from the web service with a list of group chats
    /// </summary>
    [DataContract]
    public class GroupResponse
    {
        /// <summary>
        /// Gets or sets the list of group chat conversations
        /// </summary>
        [DataMember(Name = "response")]
        public Group[] Groups { get; set; }
    }
}
