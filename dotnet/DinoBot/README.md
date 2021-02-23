# DinoBot
DinoBot is a fun, engaging bot that sends the GroupMe dinosaur emoji to your groups on GroupMe.

## How To Use
- "[n] dinos" -> The number n dinosaur emojis will be sent to the chat with a max of 100. If the original message was a reply, Dinobot will also reply to that message.
- "Hey Dinobot" -> Dinobot will respond with a Dino.
- "[Any question]" -> Dinobot will use super sophisticated AI to determine if he should answer with a dino.

## Building and Deploying DinoBot

DinoBot is designed to run as an [Azure Function](https://azure.microsoft.com/en-us/services/functions) that is triggered when messages are sent to the group in which it is installed. The instructions provided below explain how to install DinoBot with an Azure Function as the host.

There is no hard requirement to use Azure Functions, though. The majority of the code is portable to any C#-based hosting solution. Just set up your own listener, and instantiate and invoke `DinoBot` in the same manner as the [Run](https://github.com/groupme/BotExamples/blob/6b4bcbcbfe00ea493e8abac2327d122d5bf2f7b3/dotnet/DinoBot/DinoBot/DinoBot.cs#L102) method.

### Requirements

- An active Azure account
- An active GroupMe account
- Visual Studio with .NET 4.6.1

### Installation Instructions

1) Create a .NET Azure Function resource on https://portal.azure.com
2) Open https://[yourappurl].scm.azurewebsites.net
3) Click `Debug Console` then `Powershell`
4) Open `D:\home\site\wwwroot`
5) Open `Dinobot.sln` in Visual Studio and build
6) Drag-and-drop the files from `DinoBot\bin\release\net461` to this folder in your browser
7) Open new Azure Function resource
8) Open the `Functions` tab then the `Dinobot` function
9) Click `Get Function URL` and copy this value. It shoould look like this: https://[yourappurl].azurewebsites.net/api/Dinocallback/{botId}?code=[functioncode]

Your bot is now ready to use! Now, add the bot to GroupMe

1) Open https://dev.groupme.com and click `Bots` at the top
2) Click `Create Bot`
3) Choose your group and avatar. We recommend using [dino_avatar.png](dino_avatar.png)
4) Set the Azure Function URL from step 9 above as the `Callback URL`
5) Save the new bot
6) Copy the `botId` for the newly created bot
7) Click `Edit` on the bot, and replace `{botId}` in the CallbackURL with this ID, so it looks like https://[yourappurl].azurewebsites.net/api/Dinocallback/1234?code=[functioncode]

Save and enjoy! 

### Customization
You can set App Settings variables in Azure to change who can address DinoBot.

- `CanAddressDino`: A comma-delimited list of user IDs that can use the "hey dinobot" command. Should be lower case.
- `DinoAddressTrigger`: A comma-delimited list of phrases that Dinobot will respond to. Should be lower case.

These settings can be tested locally by creating a `local.settings` file in the `DinoBot` project and including them under `Values`. Do not deploy `local.settings` to Azure as it will be ignored.

## Todo
- Upgrade to Azure Functions 3
- Move shared code to a re-usable nuget package
- Make Dinobot do math, e.g. "10-3 dinos"
- Make more things configurable, like the maximum number of dinos
- More detailed tests