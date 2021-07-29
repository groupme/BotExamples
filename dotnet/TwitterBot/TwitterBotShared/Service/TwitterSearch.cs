using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using GroupMeShared.Utilities;
using TwitterBotShared.Model;

namespace TwitterBotShared.Service
{
    /// <summary>
    /// Searches Twitter for specific keywords
    /// </summary>
    public class TwitterSearch
    {
        private const string AuthUrl = "https://api.twitter.com/oauth2/token";
        private const string RequestUrl = "https://api.twitter.com/1.1/search/tweets.json?q={0}&result_type=recent&count=100";
        private const string TweetFormat = "https://twitter.com/{0}/status/{1}";

        /// <summary>
        /// Gets an authentication token for Twitter
        /// </summary>
        /// <returns>Auth token, null if failed</returns>
        public async Task<string> AuthenticateWithTwitter(string twitterAppKey, string twitterAppSecret)
        {
            string keyEncoded = HttpUtility.UrlEncode(twitterAppKey);
            string secretEncoded = HttpUtility.UrlEncode(twitterAppSecret);

            string keySecretEncoded = Base64Encode(keyEncoded + ":" + secretEncoded);
            string credentials = "Basic " + keySecretEncoded;
            string contentType = "application/x-www-form-urlencoded;charset=UTF-8";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", credentials);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", contentType);
            var values = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };
            var body = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(AuthUrl, body);
            if (response?.IsSuccessStatusCode == true)
            {
                var token = await JsonSerializer.DeserializeJsonAsync<TwitterAuth>(response);
                return token.AccessToken;
            }
            return null;
        }

        /// <summary>
        /// Gets a list of tweets for a search term
        /// </summary>
        /// <param name="token">Twitter authentication token to use to search</param>
        /// <param name="searchTerm">Term for which to search Twitter</param>
        /// <param name="sinceId">ID of the tweet to use as the starting point of the search. Null/empty will result in the most recent 100 results.</param>
        /// <returns>Twitter result data</returns>
        public async Task<List<TwitterResult>> GetTweets(string token, string searchTerm, string sinceId)
        {
            string request = RequestUrl;
            request = string.Format(RequestUrl, WebUtility.UrlEncode(searchTerm));
            if (!string.IsNullOrWhiteSpace(sinceId))
            {
                request += $"&since_id={sinceId}";
            }
            var client = new HttpClient();
            string auth = "Bearer " + token;
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", auth);
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "GroupMeTwitterBot");

            var response = await client.GetAsync(request);
            if (response?.IsSuccessStatusCode == true)
            {
                var data = await JsonSerializer.DeserializeJsonAsync<TwitterResponse>(response);
                if (data?.Statuses != null)
                {
                    var tweets = new List<TwitterResult>();
                    foreach (var status in data.Statuses)
                    {
                        if (status.Id != null && status?.User?.ScreenName != null)
                        {
                            if (status.Text?.Contains("GroupMe by Cat Eyes") != false)
                            {
                                Console.WriteLine("Skipping spam...");
                                continue;
                            }

                            // Retweets will come through as though they were new tweets.
                            // We want to skip over any retweet that had an original tweet more than five minutes ago.
                            // Rationale being, in that case, we should have posted it already.
                            //
                            // There's still potential for duplication if a tweet is retweeted in its first five minutes of existence.
                            if (status.RetweetCount > 0)
                            {
                                if (status.Retweet != null && status.Retweet.CreatedAtTime < DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)))
                                {
                                    Console.WriteLine("Skipping retweet...");
                                    continue;
                                }
                            }

                            string tweet = string.Format(TweetFormat, status.User.ScreenName, status.Id);
                            tweets.Add(new TwitterResult()
                            {
                                TweetId = status.Id,
                                TweetUrl = tweet,
                                CreationTime = status.CreatedAtTime
                            });
                        }
                    }
                    return tweets;
                }
            }
            return null;
        }

        private string Base64Encode(string stringText)
        {
            var stringTextBytes = Encoding.UTF8.GetBytes(stringText);
            return Convert.ToBase64String(stringTextBytes);
        }
    }
}