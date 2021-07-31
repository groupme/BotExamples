package main

import (
	"context"
	"flag"
	"os"
	"os/signal"
	"strings"
	"time"

	twitterbot "github.com/groupme/BotExamples/go/twitter-bot"
)

var (
	flagTweetTracks      = flag.String("tweet-tracks", "groupme,cats,burrito", "What tags to filter on. Passed as a single comma seperated string")
	flagGroupMeBotID     = flag.String("groupme-bot-id", "", "Identifier for your bot on GroupMe")
	flagTwitterAppKey    = flag.String("twitter-app-key", "", "Developer Key associated with your twitter project")
	flagTwitterAppSecret = flag.String("twitter-app-secret", "", "Developer secret associated with your twitter project")
)

func main() {
	flag.Parse()

	// setup signal aware context.
	ctx, cancel := signal.NotifyContext(context.Background(), os.Interrupt, os.Kill)
	defer cancel()

	twitterClient, _ := twitterbot.NewTwitterClient(*flagTwitterAppKey, *flagTwitterAppSecret)
	botClient, _ := twitterbot.NewBotClient(*flagGroupMeBotID)

	poster, err := twitterbot.NewTwitterPoster(twitterClient, botClient, strings.Split(*flagTweetTracks, ",")...)
	if err != nil {
		panic(err)
	}

	// Default rate limit is 450 requests in an hour, so fetch new tweets every 10 seconds
	go poster.StartListening(time.Second * 10)

	<-ctx.Done()

	err = poster.StopListening()
	if err != nil {
		panic(err)
	}
}
