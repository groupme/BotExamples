# What is it

It is a simple bot that attempts to read tweets from twitter, specified by the tweet-query passed, and write it to a given group by using a bot user.


# How to use it
To use this bot in either a docker container or locally,  you'll need the following:

1) A query for tweets set in an environment variable TWEET_QUERY
2) A groupMe bot ID set in GROUPME_BOT_ID
3) A twitter app key and secret, set in TWITTER_APP_KEY and TWITTER_APP_SECRET respectively

## Locally
1. Have go/make installed.
2. $ make run # Run in your terminal

## In a container
1. Ensure the environment variables above are set
2. Run /app/main -tweet-query=${TWEET_QUERY} -groupme-bot-id=${GROUPME_BOT_ID} -twitter-app-key=${TWITTER_APP_KEY} -twitter-app-secret=${TWITTER_APP_SECRET}
