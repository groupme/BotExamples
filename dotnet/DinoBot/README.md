# Requirements

- An active Azure account
- An active GroupMe account
- Visual Studio with .NET 4.6.1

# Installation Instructions

1) Create a .NET Azure Function resource on https://portal.azure.com
2) Open https://[yourappurl].scm.azurewebsites.net
3) Press 'Debug Console' then 'Powershell'
4) Open 'D:\home\site\wwwroot'
5) Drag-and-drop the files from DinoBot\bin\release\net461 to this folder
6) Open Dinobot.sln in Visual Studio and build
7) Open new Azure Function resource
8) Open the 'Functions' tab then 'Dinobot'
9) Click 'Get Function URL' and copy this value. It shoould look like this: https://[yourappurl].azurewebsites.net/api/Dinocallback/{botId}?code=[functioncode]

Your bot is now ready to use! Now, add a bot to GroupMe

1) Open https://dev.groupme.com and click 'Bots' at the top
2) Click 'Create Bot'
3) Choose your group and avatar. We recommend using https://powerups.s3.amazonaws.com/emoji/1/sticker/xxxhdpi/62.png
4) Set this Azure Function URL from above as the Callback URL
5) Save the new bot
6) Grab the botId for the newly created bot
7) Click 'Edit' on the bot, and replace {botId} in the CallbackURL with this ID, so it looks like https://[yourappurl].azurewebsites.net/api/Dinocallback/1234?code=[functioncode]

Save and enjoy! Dinobot can be addressed in the following ways:

- "[n] dinos" -> The number n dinosaur emojis will be sent to the chat with a max of 100. If the original message was a reply, Dinobot will also reply to that message
- "Hey Dinobot" -> Dinobot will respond with a Dino
- "[Any question]" -> Dinobot will use super sophisticated AI to determine if he should answer with a dino

# Customization
You can set App Settings variables in Azure to change who can address DinoBot.
CanAddressDino: A comma-delimited list of user IDs that can use the "hey dinobot" command
DinoAddressTrigger: A comma-delimited list of phrases that Dinobot will respond to

# Todo
- Upgrade to Azure Functions 3
- Move shared code to a re-usable nuget package
- Make Dinobot do math, e.g. "10-3 dinos"
- Make more things configurable, like the maximum number of dinos