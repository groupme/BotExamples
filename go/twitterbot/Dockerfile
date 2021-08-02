FROM golang:1.16-alpine as base 
WORKDIR /app 
RUN apk add --no-cache make 

FROM base as builder 
COPY . ./
RUN go mod download
RUN make build
RUN chmod +x /app/bin/main 

FROM base as runtime 

COPY --from=builder /app/bin/main . 
	
CMD /app/main -tweet-query=${TWEET_QUERY} -groupme-bot-id=${GROUPME_BOT_ID} -twitter-app-key=${TWITTER_APP_KEY} -twitter-app-secret=${TWITTER_APP_SECRET}
