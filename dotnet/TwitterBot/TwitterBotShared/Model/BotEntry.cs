// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.Serialization;

using GroupMeShared.Model;
using Microsoft.WindowsAzure.Storage.Table;

namespace TwitterBotShared.Model
{
    public class BotDetails
    {
        /// <summary>
        /// Helper function to combine group and bot data
        /// </summary>
        /// <param name="entries">Bots to create</param>
        /// <param name="groups">Groups the user is a part of</param>
        /// <returns>Details about the bots</returns>
        public static List<BotDetails> CreateBots(List<BotEntry> entries, List<Group> groups)
        {
            // Creates map of existing groups
            var groupMap = new Dictionary<string, Group>();
            foreach (var group in groups)
            {
                groupMap[group.Id] = group;
            }

            // For each bot entry, combine the group data into the bot data
            var bots = new List<BotDetails>();
            foreach (var entry in entries)
            {
                var bot = new BotDetails
                {
                    BotId = entry.BotId,
                    GroupId = entry.GroupId,
                    SearchTerm = entry.SearchTerm,
                    RecentId = entry.MostRecentId,
                };
                Group group;
                if (groupMap.TryGetValue(entry.GroupId, out group))
                {
                    bot.GroupName = group.GroupName;
                }
                bots.Add(bot);
            }
            return bots;
        }

        /// <summary>
        /// Gets or sets the ID of the bot
        /// </summary>
        public string BotId { get; set; }

        /// <summary>
        /// Gets or sets the name of the group the bot is in
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the group the bot is in
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the term the bot should search Twitter for
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// The most ID of the most recent tweet the bot has posted
        /// </summary>
        public string RecentId { get; set; }
    }

    /// <summary>
    /// Entry data for a created bot - designed to be stored in Azure Table Storage
    /// </summary>
    [DataContract]
    public class BotEntry : TableEntity
    {
        public BotEntry()
        {

        }

        public BotEntry(string userId, string id, string searchTerm, string groupId)
        {
            PartitionKey = "TwitterBots";
            RowKey = id;
            UserId = userId;
            SearchTerm = searchTerm;
            GroupId = groupId;
        }

        /// <summary>
        /// Gets or sets the ID of the bot
        /// </summary>
        [DataMember]
        public string BotId
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        /// <summary>
        /// Gets or sets the GroupMe access token used to create the bot
        /// </summary>
        [DataMember]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created the bot
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the group containing the bot
        /// </summary>
        [DataMember]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the Twitter term for which the bot should search
        /// </summary>
        [DataMember]
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the ID of the most recent tweet found - used as a starting point for future searches
        /// </summary>
        [DataMember]
        public string MostRecentId { get; set; }
    }
}