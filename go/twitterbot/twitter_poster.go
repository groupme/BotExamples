package twitterbot

import (
	"errors"
	"fmt"
	"log"
	"sort"
	"strings"
	"time"

	"github.com/dghubble/go-twitter/twitter"
)

// TwitterPoster attempts to construct a persistant connection to a twitter stream, filtered based on tags given to construct it.
type TwitterPoster struct {
	twitterClient  *twitter.Client
	botclient      *BotClient
	tags           []string
	tweetStream    chan (twitter.Tweet)
	stop           chan (bool)
	stopped        chan (bool)
	sinceIdHorizon int64
}

// NewTwitterPoster constructs a TwitterFilterPoster, requiring that the given twitter.Client, bot.Client, and tags are all present
func NewTwitterPoster(tc *twitter.Client, bc *BotClient, tags ...string) (*TwitterPoster, error) {
	if tc == nil {
		return nil, errors.New("Unable to construct TwitterPoster without initialized twitter client")
	} else if bc == nil {
		return nil, errors.New("Unable to construct TwitterPoster without initialized bot client")
	} else if len(tags) == 0 {
		return nil, errors.New("Unable to construct TwitterPoster without some tags present")
	}

	return &TwitterPoster{
		twitterClient: tc,
		botclient:     bc,
		tags:          tags,
		// initializing the size of the tweet stream equal to the count we anticipate an individual request to have
		tweetStream: make(chan twitter.Tweet, 10),
		stop:        make(chan bool),
	}, nil
}

// StartListening fetches tweets, sleeping for the given time duration between fetches.
func (tp *TwitterPoster) StartListening(sleepDuration time.Duration) {
	go func() {
		for {
			select {
			case <-tp.stop:
				tp.stopped <- true
				return
			default:
				tp.fetchTweets()
				time.Sleep(sleepDuration)
			}
		}
	}()

	go func() {
		for tweet := range tp.tweetStream {
			// https://blog.twitter.com/developer/en_us/topics/tips/2020/getting-to-the-canonical-url-for-a-tweet
			err := tp.botclient.SendMessage(fmt.Sprintf("https://twitter.com/twitter/status/%s", tweet.IDStr))
			if err != nil {
				log.Fatal(err)
			}

			log.Printf("Wrote tweet %s", tweet.IDStr)

			// Sleep for 5 seconds between writing tweets
			time.Sleep(time.Second * 5)
		}
	}()
}

func (tp *TwitterPoster) fetchTweets() {
	log.Printf("Fetching tweets since %d\n", tp.sinceIdHorizon)
	// https://developer.twitter.com/en/docs/twitter-api/v1/tweets/search/api-reference/get-search-tweets
	payload, response, err := tp.twitterClient.Search.Tweets(&twitter.SearchTweetParams{
		Query:      strings.Join(tp.tags, " OR "),
		SinceID:    tp.sinceIdHorizon,
		ResultType: "recent",
	})

	if err != nil {
		log.Fatal(err)
	}

	log.Printf("Fetched %d new tweets. HTTP Status=%s, Remaining Rate Limit=%s", len(payload.Statuses), response.Status, response.Header.Get("X-Rate-Limit-Remaining"))

	if len(payload.Statuses) == 0 {
		return
	}

	sort.Slice(payload.Statuses, func(i int, j int) bool {
		lCreated, _ := payload.Statuses[i].CreatedAtTime()
		rCreated, _ := payload.Statuses[j].CreatedAtTime()
		return lCreated.Before(rCreated)
	})

	if payload.Statuses[len(payload.Statuses)-1].ID > tp.sinceIdHorizon {
		tp.sinceIdHorizon = payload.Statuses[len(payload.Statuses)-1].ID
	}

	for _, tweet := range payload.Statuses {
		tp.tweetStream <- tweet
	}

}

// StopListening causes the TwitterFilterPoster to stop listening for tweets
func (tp *TwitterPoster) StopListening() {
	close(tp.tweetStream)
	tp.stop <- true
	<-tp.stopped
}
