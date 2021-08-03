package twitterbot

import (
	"errors"

	"github.com/dghubble/go-twitter/twitter"
	"golang.org/x/oauth2"
	"golang.org/x/oauth2/clientcredentials"
)

// newTwitterClient returns a configured twitter.Client using application only credentials
func NewTwitterClient(appKey string, appSecret string) (*twitter.Client, error) {
	if appKey == "" || appSecret == "" {
		return nil, errors.New("Unable to instantiate oauth2 twitter client without app key and app secret")
	}

	config := &clientcredentials.Config{
		ClientID:     appKey,
		ClientSecret: appSecret,
		TokenURL:     "https://api.twitter.com/oauth2/token",
	}

	return twitter.NewClient(config.Client(oauth2.NoContext)), nil
}
