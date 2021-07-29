using GroupMeShared.Model;
using GroupMeShared.Utilities;
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GroupMeShared.Service
{
    /// <summary>
    /// Used to access some basic functions of the GroupMe service
    /// </summary>
    public class GroupMeService
    {
        private const string PageParameterName = "page";
        private const string PerPageParameterName = "per_page";
        private const string AccessTokenHeader = "X-Access-Token";
        private const string UserAgentHeader = "User-Agent";
        private const string UserAgentHeaderValue = "twitterbot";

        private const string GroupsUrl = "https://api.groupme.com/v3/groups";
        private const string UserUrl = "https://api.groupme.com/v3/users/me";

        /// <summary>
        /// Gets the profile for the user
        /// </summary>
        /// <param name="accessToken">Access token to use to authenticate the request</param>
        /// <returns>User profile data for the logged in data</returns>
        public static async Task<User> GetUser(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("accessToken cannot be null");
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(UserAgentHeader, UserAgentHeaderValue);
            client.DefaultRequestHeaders.Add(AccessTokenHeader, accessToken);
            try
            {
                HttpResponseMessage response = await client.GetAsync(UserUrl);
                if (response?.IsSuccessStatusCode == true)
                {
                    var user = await JsonSerializer.DeserializeJsonAsync<UserEnvelope>(response);
                    return user?.Response;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new AuthorizationException();
                }
            }
            catch (AuthorizationException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Attempts to retrieve the full list of groups for the user
        /// </summary>
        /// <param name="accessToken">Access token for the user</param>
        /// <returns>List of groups, null if failed</returns>
        public static async Task<List<Group>> GetGroupsForUser(string accessToken)
        {
            var groups = new List<Group>();
            bool moreGroups = true;
            int page = 1;
            while (moreGroups)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[PageParameterName] = page.ToString();
                parameters[PerPageParameterName] = "100";

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(UserAgentHeader, UserAgentHeaderValue);
                client.DefaultRequestHeaders.Add(AccessTokenHeader, accessToken);
                string url = string.Format("{0}?{1}", GroupsUrl, BuildParameterString(parameters));
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response?.IsSuccessStatusCode == true)
                    {
                        var groupData = await JsonSerializer.DeserializeJsonAsync<GroupResponse>(response);
                        if (groupData?.Groups?.Any() == true)
                        {
                            groups.AddRange(groupData.Groups);
                            ++page;
                        }
                        else
                        {
                            moreGroups = false;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new AuthorizationException();
                    }
                }
                catch (AuthorizationException)
                {
                    throw;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return groups;
        }

        /// <summary>
        /// Builds a list of parameters to add to a URL
        /// </summary>
        /// <param name="parameters">Parameters to convert to URL format</param>
        /// <returns>Parameters in the following format: 'param1=value1&amp;param2=value2&amp;paramn=valuen'</returns>
        private static string BuildParameterString(Dictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;

            foreach (string key in parameters.Keys)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append("&");
                }

                sb.AppendFormat("{0}={1}", key, WebUtility.UrlEncode(parameters[key]));
            }

            return sb.ToString();
        }
    }
}
