// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GroupMeShared.Model
{
    /// <summary>
    /// Represents an attachment to a message
    /// </summary>
    [DataContract]
    public class Attachment
    {
        /// <summary>
        /// An attachment type representing an image
        /// </summary>
        public const string ImageType = "image";

        /// <summary>
        /// An attachment type representing a video
        /// </summary>
        public const string VideoType = "video";

        /// <summary>
        /// An attachment type representing an event
        /// </summary>
        public const string EventType = "event";

        /// <summary>
        /// An attachment type representing a location
        /// </summary>
        public const string LocationType = "location";

        /// <summary>
        /// An attachment type representing user mentions.
        /// </summary>
        public const string MentionsType = "mentions";

        /// <summary>
        /// An attachment type representing an emoji
        /// </summary>
        public const string Emoji = "emoji";

        /// <summary>
        /// An attachment type representing a file
        /// </summary>
        public const string FileType = "file";

        /// <summary>
        /// An attachment type representing a reply
        /// </summary>
        public const string ReplyType = "reply";

        /// <summary>
        /// An image that's been linked into the message from an external service
        /// </summary>
        public const string LinkedImageType = "linked_image";

        /// <summary>
        /// An attachment type representing a poll
        /// </summary>
        public const string PollType = "poll";

        /// <summary>
        /// Gets or sets the type of attachment (picture, video, location, or emoji)
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets placeholder text for emoji
        /// </summary>
        [DataMember(Name = "placeholder", EmitDefaultValue = false)]
        public string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the URL to the attachment (image/video)
        /// </summary>
        [DataMember(Name = "url", EmitDefaultValue = false)]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the URL to the source image (image)
        /// </summary>
        [DataMember(Name = "source_url", EmitDefaultValue = false)]
        public string SourceUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to a preview of the attachment (video)
        /// </summary>
        [DataMember(Name = "preview_url", EmitDefaultValue = false)]
        public string PreviewUrl { get; set; }

        /// <summary>
        /// Gets or sets the Latitude of the attached location
        /// </summary>
        [DataMember(Name = "lat", EmitDefaultValue = false)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude of the attached location
        /// </summary>
        [DataMember(Name = "lng", EmitDefaultValue = false)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the name of the attached location
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID to the attached event
        /// </summary>
        [DataMember(Name = "event_id", EmitDefaultValue = false)]
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the view of the event
        /// </summary>
        [DataMember(Name = "view", EmitDefaultValue = false)]
        public string EventView { get; set; }

        /// <summary>
        /// Gets or sets the user identifiers for any mentions.
        /// </summary>
        [DataMember(Name = "user_ids", EmitDefaultValue = false)]
        public List<string> UserIds { get; set; }

        /// <summary>
        /// Gets or sets the ID of the attached file.
        /// </summary>
        [DataMember(Name = "file_id", EmitDefaultValue = false)]
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the attached poll.
        /// </summary>
        [DataMember(Name = "poll_id", EmitDefaultValue = false)]
        public string PollId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the message being replied to
        /// </summary>
        [DataMember(Name = "reply_id", EmitDefaultValue = false)]
        public string ReplyId { get; set; }

        /// <summary>
        /// Gets or sets the base ID of the message thread being replied to
        /// </summary>
        [DataMember(Name = "base_reply_id", EmitDefaultValue = false)]
        public string BaseReplyId { get; set; }

        /// <summary>
        /// Gets or sets the emoji character map
        /// </summary>
        [DataMember(Name = "charmap", EmitDefaultValue = false)]
        public int[][] Charmap { get; set; }

        /// <summary>
        /// Gets or sets the mention location map
        /// </summary>
        [DataMember(Name = "loci", EmitDefaultValue = false)]
        public int[][] LocationMap { get; set; }
    }
}