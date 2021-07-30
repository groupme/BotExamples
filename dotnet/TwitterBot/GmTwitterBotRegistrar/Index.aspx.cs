// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;

using GroupMeBots.Service;
using GroupMeShared.Service;
using TwitterBotShared.Model;
using TwitterBotShared.Service;

namespace GmTwitterBotRegistrar
{
    public partial class Index : Page
    {
        private const string TwitterBotName = "TwitterBot";
        private const string BotPostUrl = "https://api.groupme.com/v3/bots/post";

        protected async void Page_Load(object sender, EventArgs e)
        {
            // No procesing necessary for postbacks
            if (Page.IsPostBack)
            {
                return;
            }

            // Look for the token in the session, if they're already authenticated, or in the QueryString for the callback from oauth.
            string token = Session["Token"] as string;
            if (string.IsNullOrEmpty(token))
            {
                token = Request.QueryString["access_token"];
                Session["Token"] = token;
            }

            if (string.IsNullOrEmpty(token))
            {
                // Not logged in - prompt to connect
                ShowLoginView();
            }
            else if (string.IsNullOrEmpty(Session["UserId"] as string))
            {
                // Logged in but no data - try to get their profile data

                try
                {
                    var user = await GroupMeService.GetUser(token);
                    if (user != null)
                    {
                        Session["UserId"] = user.Id;
                        Session["Name"] = user.Name;
                        await ShowMainView();
                    }
                }
                catch (AuthorizationException)
                {
                    RedirectToLogin();
                }
            }
        }

        private void ShowLoginView()
        {
            MainPanel.Visible = false;
            LogInPanel.Visible = true;
        }

        private async Task ShowMainView()
        {
            MainPanel.Visible = true;
            LogInPanel.Visible = false;
            NameLabel.Text = $"Hello, {(Session["Name"] as string) ?? "Unknown person"}! Let's make some TwitterBots!";
            await LoadGroups();
        }

        private async Task LoadGroups()
        {
            Result.Text = "Loading your groups...";
            string token = Session["Token"] as string;
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            try
            {
                var groups = await GroupMeService.GetGroupsForUser(token);
                if (groups != null)
                {
                    var bots = (await AzureStorage.GetBotsForUserAsync(Session["UserId"] as string)).ToList();
                    if (bots.Any())
                    {
                        var botData = BotDetails.CreateBots(bots, groups);
                        ExistingBots.DataSource = botData;
                        ExistingBots.DataBind();
                        ExistingBots.Visible = true;
                    }
                    else
                    {
                        ExistingBots.Visible = false;
                    }

                    Result.Text = "";
                    GroupList.DataSource = groups.OrderByDescending(g => g.UpdatedFromEpoch);
                    GroupList.DataBind();
                }
                else
                {
                    Result.Text = "Loading groups failed!";
                }
            }
            catch (AuthorizationException)
            {
                RedirectToLogin();
            }
        }

        private void RedirectToLogin()
        {
            Result.Text = "Unauthorized! Please log in again.";
            Session["Token"] = null;
            Session["UserId"] = null;
            Session["Name"] = null;
            ShowLoginView();
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            Response.Redirect(Environment.GetEnvironmentVariable("TwitterBotOauthRedirect"));
        }

        protected async void CreateButton_Click(object sender, EventArgs e)
        {
            Result.Text = "";

            string token = Session["Token"] as string;
            if (string.IsNullOrEmpty(token))
            {
                Result.Text = "No user token!";
                return;
            }
            string groupId = GroupList.SelectedValue;
            if (string.IsNullOrEmpty(groupId))
            {
                Result.Text = "No Group ID!";
                return;
            }
            string searchTerm = TwitterSearchTerm.Text;
            if (string.IsNullOrEmpty(searchTerm))
            {
                Result.Text = "No search term!";
                return;
            }
            string userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                Result.Text = "No User ID!";
                return;
            }

            CreateButton.Enabled = false;
            Result.Text = "Creating bot...";
            try
            {
                IBotPoster botPoster = new BotPoster(BotPostUrl);
                IBotCreator botCreator = new BotCreator(botPoster);
                var bot = await botCreator.CreateTwitterBot(token, groupId, TwitterBotName, searchTerm);
                if (bot == null)
                {
                    Result.Text = "Bot creation failed";
                }
                else
                {
                    Result.Text = "New Bot created! ID: " + bot.BotId + " - Storing...";
                    var entry = new BotEntry(userId, bot.BotId, TwitterSearchTerm.Text, groupId);
                    bool result = await AzureStorage.AddEntryAsync(entry);
                    if (result)
                    {
                        Result.Text = "Bot stored!";
                    }
                    else
                    {
                        Result.Text = "Bot storage failed!";
                    }
                }
            }
            catch (AuthorizationException)
            {
                RedirectToLogin();
            }
            CreateButton.Enabled = true;
        }
    }
}