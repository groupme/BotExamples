package twitterbot

import (
	"errors"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"

	"github.com/Azure/go-autorest/autorest"
)

// BotClient is a simple wrapper for writing messages on GroupMe
type BotClient struct {
	botID string
}

// NewBotClient returns a configured BotClient suitable for use
func NewBotClient(botID string) (*BotClient, error) {
	if botID == "" {
		return nil, errors.New("Can't instantiate a bot client with no botID")
	} else {
		return &BotClient{
			botID: botID,
		}, nil
	}
}

// botMessage is a simplified message body for some text sent on behalf of a bot.
type botMessage struct {
	BotID string `json:"bot_id"`
	Text  string `json:"text"`
}

// SendMessage posts a message on behalf of a bot. If the message is empty, an error will be returned
func (bc *BotClient) SendMessage(message string) error {
	req, err := autorest.Prepare(&http.Request{},
		autorest.WithBaseURL("https://api.groupme.com/v3/bots/post"),
		autorest.WithMethod("POST"),
		autorest.WithHeader("Content-Type", "application/json"),
		autorest.WithJSON(botMessage{
			BotID: bc.botID,
			Text:  message,
		}),
	)
	if err != nil {
		return fmt.Errorf("Unable to construct request to send message on behalf of bot: %w", err)
	}

	resp, err := autorest.Send(req)
	if err != nil {
		return fmt.Errorf("Error sending request to create a message: %w", err)
	}

	if resp.StatusCode != http.StatusAccepted {
		bytes, _ := ioutil.ReadAll(resp.Body)
		log.Fatal("Bad response code from GroupMe:", string(bytes))
		err := fmt.Errorf("Unexpected HTTP status code when creating message: %d", resp.StatusCode)
		return err
	}

	return nil
}
