using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TwitterBotShared.Model;

namespace TwitterBotShared.Service
{
    /// <summary>
    /// Stores and retrieves data from Azure table storage
    /// </summary>
    public class AzureStorage
    {   
        /// <summary>
        /// Gets all bots currently stored in table storage
        /// </summary>
        /// <returns>List of current bots</returns>
        public static async Task<IEnumerable<BotEntry>> GetExistingBotsAsync()
        {
            var table = await GetBotTableAsync();
            var query = new TableQuery<BotEntry>();
            var result = table.ExecuteQuery(query);
            return result;
        }

        public static async Task<IEnumerable<BotEntry>> GetBotsForUserAsync(string userId)
        {
            var table = await GetBotTableAsync();
            var query = new TableQuery<BotEntry>().Where(TableQuery.GenerateFilterCondition(nameof(BotEntry.UserId), QueryComparisons.Equal, userId));
            var result = table.ExecuteQuery(query);
            return result;
        }

        /// <summary>
        /// Adds or updates a bot entry in Azure table storage
        /// </summary>
        /// <param name="entry">Entry to add</param>
        /// <returns>True if successfully stored in Azure</returns>
        public static async Task<bool> AddEntryAsync(BotEntry entry)
        {
            TableOperation insertOrReplace = TableOperation.InsertOrReplace(entry);
            var table = await GetBotTableAsync();
            var result = await table.ExecuteAsync(insertOrReplace);
            return result?.HttpStatusCode == 204;
        }

        /// <summary>
        /// Retrieves a table from Azure table storage
        /// </summary>
        /// <returns>Created/opened table</returns>
        private static async Task<CloudTable> GetBotTableAsync()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=gmtwitterbots;AccountKey=Yah8b5K2MrmXkeQR6TizahzQnQ/KvN4RATa3yjLpZL7p3mpXelCOxdcy6LRPEcCmu5ceCgDD7OU0kLdOYEjooQ==;EndpointSuffix=core.windows.net");//Environment.GetEnvironmentVariable("TwitterBotStorageKey"));
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference("twitterbots");
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
