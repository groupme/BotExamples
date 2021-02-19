// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using DinoBot;
using GroupMeBots.Service;
using GroupMeShared.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DinoTests
{
    [TestClass]
    public class DinobotTests
    {
        private const string TestBotId = "123";
        private const string TestMessageWithDinos =
            "{" +
            "  'group_id': '1', " + 
            "  'text': '3 dinos' " +
            "}";


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMissingBotId()
        {
            var botPoster = new Mock<IBotPoster>().Object;
            var bot = new Dinobot(null, botPoster);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMissingBotPoster()
        {
            var bot = new Dinobot(TestBotId, null);
        }

        [TestMethod]
        public async Task TestParsingNoMessage()
        {
            var botPoster = new Mock<IBotPoster>().Object;
            var bot = new Dinobot(TestBotId, botPoster);

            bool parsed = await bot.ParseIncomingRequestAsync(null);
            Assert.IsFalse(parsed);
            Assert.IsNull(bot.Message);
        }

        [TestMethod]
        public async Task TestParsingMessage()
        {
            var botPoster = new Mock<IBotPoster>().Object;
            var bot = new Dinobot(TestBotId, botPoster);

            var message = new HttpRequestMessage
            {
                Content = new StringContent(TestMessageWithDinos)
            };

            bool parsed = await bot.ParseIncomingRequestAsync(message);
            Assert.IsTrue(parsed);
            Assert.IsNotNull(bot.Message);
            Assert.AreEqual("1", bot.Message.GroupId);
            Assert.AreEqual("3 dinos", bot.Message.Text);
        }

        [TestMethod]
        public async Task TestMessageNoMessage()
        {
            var botPoster = new Mock<IBotPoster>().Object;
            var bot = new Dinobot(TestBotId, botPoster);

            bool parsed = await bot.ProcessMessageAsync();
            Assert.IsFalse(parsed);
        }

        [TestMethod]
        public async Task TestMessageIsDm()
        {
            var botPoster = new Mock<IBotPoster>().Object;
            var bot = new Dinobot(TestBotId, botPoster);

            var message = new MessageItem
            {
                MessageId = "1",
                ChatId = "1+2",
                RecipientId = "2",
                Text = "3 dinos"
            };
            bot.Message = message;

            bool parsed = await bot.ProcessMessageAsync();
            Assert.IsFalse(parsed);
        }

        [TestMethod]
        public async Task TestMessageHasDinosServerFail()
        {
            var botPoster = new Mock<IBotPoster>();
            botPoster
                .Setup(b => b.PostEmojiAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(System.Net.HttpStatusCode.BadRequest));
            var bot = new Dinobot(TestBotId, botPoster.Object);

            var message = new MessageItem
            {
                MessageId = "1",
                GroupId = "1",
                Text = "3 dinos"
            };
            bot.Message = message;

            bool sent = await bot.ProcessMessageAsync();
            Assert.IsFalse(sent);
        }

        [TestMethod]
        public async Task TestMessageHasDinos()
        {
            var botPoster = new Mock<IBotPoster>();
            botPoster
                .Setup(b => b.PostEmojiAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(System.Net.HttpStatusCode.Accepted));
            var bot = new Dinobot(TestBotId, botPoster.Object);

            var message = new MessageItem
            {
                MessageId = "1",
                GroupId = "1",
                Text = "3 dinos"
            };
            bot.Message = message;

            bool sent = await bot.ProcessMessageAsync();
            Assert.IsTrue(sent);
        }

        [TestMethod]
        public async Task TestMessageHasDinosMiddle()
        {
            var botPoster = new Mock<IBotPoster>();
            botPoster
                .Setup(b => b.PostEmojiAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(System.Net.HttpStatusCode.Accepted));
            var bot = new Dinobot(TestBotId, botPoster.Object);

            var message = new MessageItem
            {
                MessageId = "1",
                GroupId = "1",
                Text = "so cool 3 dinos yay"
            };
            bot.Message = message;

            bool sent = await bot.ProcessMessageAsync();
            Assert.IsTrue(sent);
        }

        [TestMethod]
        public async Task TestMessageHasAddressCaps()
        {
            var botPoster = new Mock<IBotPoster>();
            botPoster
                .Setup(b => b.PostEmojiAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(System.Net.HttpStatusCode.Accepted));
            var bot = new Dinobot(TestBotId, botPoster.Object);

            var message = new MessageItem
            {
                MessageId = "1",
                GroupId = "1",
                Text = "HEY DINOBOT"
            };
            bot.Message = message;

            bool sent = await bot.ProcessMessageAsync();
            Assert.IsTrue(sent);
        }

        [TestMethod]
        public async Task TestMessageHasAddressMiddle()
        {
            var botPoster = new Mock<IBotPoster>();
            botPoster
                .Setup(b => b.PostEmojiAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(System.Net.HttpStatusCode.Accepted));
            var bot = new Dinobot(TestBotId, botPoster.Object);

            var message = new MessageItem
            {
                MessageId = "1",
                GroupId = "1",
                Text = "Oh hey dinobot what's up?"
            };
            bot.Message = message;

            bool sent = await bot.ProcessMessageAsync();
            Assert.IsTrue(sent);
        }

        [TestMethod]
        public async Task TestMessageHasAddressServerFail()
        {
            var botPoster = new Mock<IBotPoster>();
            botPoster
                .Setup(b => b.PostEmojiAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(System.Net.HttpStatusCode.BadRequest));
            var bot = new Dinobot(TestBotId, botPoster.Object);

            var message = new MessageItem
            {
                MessageId = "1",
                GroupId = "1",
                Text = "hey dinobot"
            };
            bot.Message = message;

            bool sent = await bot.ProcessMessageAsync();
            Assert.IsFalse(sent);
        }

        [TestMethod]
        public async Task TestMessageNoDinos()
        {
            var botPoster = new Mock<IBotPoster>();
            botPoster
                .Setup(b => b.PostEmojiAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(System.Net.HttpStatusCode.Accepted));
            var bot = new Dinobot(TestBotId, botPoster.Object);

            var message = new MessageItem
            {
                MessageId = "1",
                GroupId = "1",
                Text = "None here"
            };
            bot.Message = message;

            bool sent = await bot.ProcessMessageAsync();
            Assert.IsFalse(sent);
        }
    }
}
