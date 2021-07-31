build:
	go build -o bin/main cmd/main.go

run:
	go run cmd/main.go -tweet-query=${TWEET_QUERY} -groupme-bot-id=${GROUPME_BOT_ID} -twitter-app-key=${TWITTER_APP_KEY} -twitter-app-secret=${TWITTER_APP_SECRET}

container: 
	docker build -t groupme/twitter-bot .