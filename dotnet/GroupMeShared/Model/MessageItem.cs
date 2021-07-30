// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Runtime.Serialization;

namespace GroupMeShared.Model
{
    /// <summary>
    /// A generic message item from the server - support for group and direct messages
    /// </summary>
    [DataContract]
    public class MessageItem
    {
        /// <summary>
        /// Gets or sets the the server ID for this message
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the recipient of this message (DM only)
        /// </summary>
        [DataMember(Name = "recipient_id", EmitDefaultValue = false)]
        public string RecipientId { get; set; }

        /// <summary>
        /// Gets or sets the source GUID for the message
        /// </summary>
        [DataMember(Name = "source_guid", EmitDefaultValue = false)]
        public string SourceGuid { get; set; }

        /// <summary>
        /// Gets or sets the time (since epoch) at which this message was created
        /// </summary>
        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public long CreatedAtEpoch { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user that sent the message
        /// </summary>
        [DataMember(Name = "user_id", EmitDefaultValue = false)]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the group to which this message belongs (group only)
        /// </summary>
        [DataMember(Name = "group_id", EmitDefaultValue = false)]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the user who sent this message
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the URL to the avatar of the user who sent this message
        /// </summary>
        [DataMember(Name = "avatar_url", EmitDefaultValue = false)]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets the Text of the message
        /// </summary>
        [DataMember(Name = "text", EmitDefaultValue = false)]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message was sent by the system or not
        /// </summary>
        [DataMember(Name = "system", EmitDefaultValue = false)]
        public bool IsSystem { get; set; }

        /// <summary>
        /// Gets or sets the ID of the system that sent the message
        /// </summary>
        [DataMember(Name = "sender_id", EmitDefaultValue = false)]
        public string SystemSenderId { get; set; }

        /// <summary>
        /// Gets or sets the type of system that sent the message
        /// </summary>
        [DataMember(Name = "sender_type", EmitDefaultValue = false)]
        public string SystemSenderType { get; set; }

        /// <summary>
        /// Gets or sets the list of users that have liked this message
        /// </summary>
        [DataMember(Name = "favorited_by", EmitDefaultValue = false)]
        public string[] LikedBy { get; set; }

        /// <summary>
        /// Gets or sets the list of attachments for this message
        /// </summary>
        [DataMember(Name = "attachments")]
        public Attachment[] Attachments { get; set; }

        /// <summary>
        /// Gets or sets a timestamp for gallery items (used for pagination purposes)
        /// </summary>
        [DataMember(Name = "gallery_ts")]
        public string GalleryTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the chat id for this message.
        /// The chat_id field is only sent in push messages, not when the list of messages for a conversation is retrieved from the server.
        /// For direct conversations, the chat_id field has the format "[UserId1]+[UserId2]", with UserId1 &lt; UserId2.
        /// </summary>
        [DataMember(Name = "chat_id", EmitDefaultValue = false)]
        public string ChatId { get; set; }

        /// <summary>
        /// Attempts to retrieve the message to which this message is replying
        /// </summary>
        /// <returns>Attachment representing the message being replied to, null if no attachment</returns>
        public Attachment GetExistingReply()
        {
            return Attachments?.FirstOrDefault(a => a.Type == Attachment.ReplyType);
        }
    }
}