// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using GroupMeBot.Model;
using static GroupMeBot.Model.CreateBotResponse;
using GroupMeShared.Service;
using GroupMeShared.Utilities;

namespace GroupMeBots.Service
{
    public class BotCreator : IBotCreator
    {   
        private const string TwitterAvatarUrl = "https://gmbots.azurewebsites.net/twitter.png";
        private const string BotCreationUrl = "https://api.groupme.com/v3/bots?token={0}";
        private const int BotCreatedCode = 201;
        private const string WelcomeText = "Hello! I'm TwitterBot. I'll be searching Twitter every five minutes for references of '{0}'. If I find anything, I'll post it here!";

        private IBotPoster _botPoster;

        public BotCreator(IBotPoster botPoster)
        {
            _botPoster = botPoster;
        }

        /// <summary>
        /// Creates a Twitter bot in GroupMe
        /// </summary>
        /// <param name="accessToken">GroupMe access token with which to create the bot</param>
        /// <param name="groupId">ID of the group in which to create the bot</param>
        /// <param name="botname">Name of the bot you are creating</param>
        /// <param name="searchTerm">Term to use to search Twitter with the bot</param>
        /// <returns>New bot details, null if failed</returns>
        public async Task<BotData> CreateTwitterBot(string accessToken, string groupId, string botname, string searchTerm)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(searchTerm) || string.IsNullOrEmpty(botname))
            {
                throw new ArgumentException();
            }

            var request = new CreateBotRequest
            {
                Bot = new CreateBotRequest.BotData
                {
                    Name = $"{botname} {searchTerm}",
                    GroupId = groupId,
                    AvatarUrl = TwitterAvatarUrl
                }
            };

            using (StringContent content = JsonSerializer.SerializeToJson(request))
            {
                var client = new HttpClient();
                string requestUrl = string.Format(BotCreationUrl, accessToken);
                HttpResponseMessage response = await client.PostAsync(requestUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var newBot = await JsonSerializer.DeserializeJsonAsync<CreateBotResponse>(response);
                    if (newBot?.ResponseMeta?.Code == BotCreatedCode)
                    {
                        await _botPoster.PostAsync(string.Format(WelcomeText, searchTerm), newBot.Response.Bot.BotId);
                        return newBot?.Response?.Bot;
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new AuthorizationException();
                }
            }

            return null;
        }
    }
}