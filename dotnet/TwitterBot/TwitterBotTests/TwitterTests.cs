// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TwitterBotTests
{
    [TestClass]
    public class TwitterTests
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        [TestMethod]
        public async Task SearchTwitter()
        {
            string token = await TwitterSearch.AuthenticateWithTwitter();
            Assert.IsNotNull(token);
            List<TwitterResult> tweets = await TwitterSearch.GetTweets(token, "GroupMe", null);
            Assert.IsNotNull(tweets);
            Assert.IsTrue(tweets.Count > 0);
        }

        [TestMethod]
        public async Task AuthWithTwitter()
        {
            string token = await TwitterSearch.AuthenticateWithTwitter();
            Assert.IsNotNull(token);
        }
    }
}
