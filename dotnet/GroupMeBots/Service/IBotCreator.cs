// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//
using System.Threading.Tasks;
using static GroupMeBot.Model.CreateBotResponse;

namespace GroupMeBots.Service
{
    public interface IBotCreator
    {
        /// <summary>
        /// Creates a Twitter bot in GroupMe
        /// </summary>
        /// <param name="accessToken">GroupMe access token with which to create the bot</param>
        /// <param name="groupId">ID of the group in which to create the bot</param>
        /// <param name="botname">Name of the bot you are creating</param>
        /// <param name="searchTerm">Term to use to search Twitter with the bot</param>
        /// <returns>New bot details, null if failed</returns>
        Task<BotData> CreateTwitterBot(string accessToken, string groupId, string botName, string searchTerm);
    }
}
