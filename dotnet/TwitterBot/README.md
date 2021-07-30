# TwitterBot
TwitterBot periodically searches Twitter for specified terms and posts the results to your groups.

This project includes BotRunner, a console application that runs on a schedule to search for Tweets and post them, and TwitterBotRegistrar, a web portal that lets you authenticate with GroupMe OAuth to retrieve your group list and create a TwitterBot in your groups.

###  Technologies used:
- C# for all projects
- ASP.NET for TwitterBotRegistrar portal
- Azure Storage to store the metadata about your TwitterBots
- Azure hosting to run the portal as well as host the scheduled BotRunner
- GroupMe OAuth for authentication
- Twitter's public API for searching for tweets

### What you need:
- An [Azure Account](https://portal.azure.com) with 1 Windows host and 1 Azure Storage resource
- [Twitter API Key](https://developer.twitter.com/)
- A [GroupMe](https://www.groupme.com) account

## Deployment
1. Sign in to your Azure account at https://portal.azure.com
2. Create a new Azure Storage resource
   - Create Resource -> Storage -> Storage Account (classic)
   - Choose your subscription, resource group, and name
   - A US-based region is preferable
   - Standard performance is fine
   - After its created, click Go to Resource
   - Go to Configuration and change minimum TLS version to 1.0
   - Click 'Access Keys' and grab your connection string for later
3. Create an Azure host resource
   - Create Resource -> Web App
   - Ideally choose the same Resource Group as step 2
   - Give your app a name/domain
   - Choose code, and .NET 4.8 as your stack
   - Prefer a US based region
   - Choose your service plan - free should be fine
   - You don't need AppInsights
   - After its created, click Go to Resource
4. Create your GroupMe OAuth app
   - Open https://dev.groupme.com
   - Log in with your GroupMe credentials
   - Click 'Applications' at the top
   - Click 'Create Application'
   - Choose a name for your app. This name will be displayed with people sign in with OAuth.
   - Enter a callback URL. This will be from the resource you created in step 3. If your resource was 'mytwitterbot.azure.com', the callback URL to enter here will be https://mytwitterbot.azure.com/index.aspx
   - Fill out the required information, accept the terms, and press Create
   - Take note of your Redirect URL
5. Configure your Twitter account
   - Open https://developer.twitter.com
   - Sign in with your Twitter account
   - Follow the steps to apply for a Twitter API account
   - Once you have an API account, create a new project
   - Take note of your API key and API secret in the settings shown after
6. Configure your Azure host
   - Go back to the resource your configured in step 3
   - Click 'Configuration'
   - Add the following Application settings:
   - TwitterApiKey: <API key from step 5>
   - TwitterApiSecret: <API Secret from step 5>
   - TwitterBotOauthRedirect: <OAuth redirect URI from step 4>
   - TwitterBotStorageKey: <Azure Storage connection string from step 2>
7. Deploy the project to Azure
   - In Visual Studio, right click the TwitterBotRegistrar project and choose 'Publish'
   - Sign in with your Azure account
   - Select publish to Azure App Service
   - Select your App Service from step 3
   - Click Publish

## TO DO
- Migrate from Azure Storage V1 to Azure Storage V2 and TLS 1.2
- Create an ARM template for easier deployment
- Make the bot management page better - remote TwitterBots, edit the search terms
- Add the test project back in (they were broken and not fixed when migrating to the common framework here, so they don't compile or run)