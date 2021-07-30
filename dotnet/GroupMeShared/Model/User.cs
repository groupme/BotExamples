// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Serialization;

namespace GroupMeShared.Model
{
    /// <summary>
    /// The JSON object for a GroupMe user
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        [DataMember(Name = "avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        [DataMember(Name = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the time, from epoch, at which this user was created
        /// </summary>
        [DataMember(Name = "created_at")]
        public long CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the time, since epoch, at which this user was last updated
        /// </summary>
        [DataMember(Name = "updated_at")]
        public long UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the phone number for the user
        /// </summary>
        [DataMember(Name = "phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the user's locale
        /// </summary>
        [DataMember(Name = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sms is enabled for this user
        /// </summary>
        [DataMember(Name = "sms")]
        public bool? SmsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Facebook is connected to this account or not
        /// </summary>
        [DataMember(Name = "facebook_connected")]
        public bool FacebookConnected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Twitter is connected to this account or not
        /// </summary>
        [DataMember(Name = "twitter_connected")]
        public bool TwitterConnected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is verified with the system
        /// </summary>
        [DataMember(Name = "verified")]
        public bool? IsVerified { get; set; }

        /// <summary>
        /// Gets or sets the default group notification sound for the user
        /// </summary>
        [DataMember(Name = "group_notification_sound")]
        public string GroupNotificationSound { get; set; }

        /// <summary>
        /// Gets or sets the default dm notification sound for the user
        /// </summary>
        [DataMember(Name = "dm_notification_sound")]
        public string DmNotificationSound { get; set; }

        /// <summary>
        /// Gets or sets the time, since epoch, for which this user start to receive notifications again
        /// </summary>
        [DataMember(Name = "sleep_until")]
        public string MuteUntil { get; set; }

        /// <summary>
        /// Gets or sets the URL to share the user's contact information
        /// </summary>
        [DataMember(Name = "share_url")]
        public string SharingLink { get; set; }

        /// <summary>
        /// Gets or sets the URL to the QR code to share the user's contact information
        /// </summary>
        [DataMember(Name = "share_qr_code_url")]
        public string SharingQrCode { get; set; }

        /// <summary>
        /// Gets or sets user tags
        /// </summary>
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
    }
}
