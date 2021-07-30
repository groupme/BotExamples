// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using GroupMeBots.Service;
using TwitterBotShared.Service;
using TwitterBotShared.Model;

namespace JobRunner
{
    /// <summary>
    /// Contains methods for running bot operations
    /// </summary>
    public class BotRunner
    {
        private const string BotPostUrl = "https://api.groupme.com/v3/bots/post";

        private TwitterSearch _twitter;

        public BotRunner(TwitterSearch twitter)
        {
            _twitter = twitter;
        }

        /// <summary>
        /// Retrieves all bots from storage and processes them
        /// </summary>
        /// <returns><Async task/returns>
        public async Task PostAllBotsAsync()
        {
            IBotPoster poster = new BotPoster(BotPostUrl);

            var bots = await AzureStorage.GetExistingBotsAsync();
            if (bots?.Any() != true)
            {
                Console.WriteLine("No bots to process");
                return;
            }

            string twitterAppKey = Environment.GetEnvironmentVariable("TwitterAppKey");
            string twitterAppSecret = Environment.GetEnvironmentVariable("TwitterAppSecret");

            var twitterToken = await _twitter.AuthenticateWithTwitter(twitterAppKey, twitterAppSecret);
            if (string.IsNullOrWhiteSpace(twitterToken))
            {
                Console.WriteLine("Failure retrieving Twitter auth token");
                return;
            }

            foreach (var bot in bots)
            {
                Console.WriteLine($"Processing bot {bot.BotId} - Searching \"{bot.SearchTerm}\"...");
                string result = await SearchAndPost(poster, bot, twitterToken);
                if (result != null)
                {
                    Console.WriteLine($"Updating latest tweet to {result}");
                    bot.MostRecentId = result;
                    await AzureStorage.AddEntryAsync(bot);
                    Console.WriteLine("Done");
                }
            }
        }

        /// <summary>
        /// Searches twitter for the bot's search term and posts the result to GroupMe
        /// </summary>
        /// <param name="data">Bot data to process</param>
        /// <param name="twitterToken">Auth token for searching Twitter</param>
        /// <returns>ID of the most recent tweet retrieved</returns>
        public async Task<string> SearchAndPost(IBotPoster poster, BotEntry data, string twitterToken)
        {
            List<TwitterResult> tweets = await _twitter.GetTweets(twitterToken, data.SearchTerm, data.MostRecentId);
            if (tweets?.Count > 0)
            {
                TwitterResult newestTweet = null;
                Console.WriteLine($"Found {tweets.Count}. Posting to GroupMe...");
                foreach (var tweet in tweets)
                {
                    var result = await poster.PostAsync(tweet.TweetUrl, data.BotId);
                    if (result != HttpStatusCode.Created)
                    {
                        Console.WriteLine("Failure posting tweet, exiting");
                        break;
                    }

                    // Keep track of the most recent tweet
                    if (newestTweet == null || tweet.CreationTime > newestTweet.CreationTime)
                    {
                        newestTweet = tweet;
                    }
                }

                return newestTweet?.TweetId;
            }
            else
            {
                Console.WriteLine("No tweets found");
                return null;
            };
        }
    }
}
