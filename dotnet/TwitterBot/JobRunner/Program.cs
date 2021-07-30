// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using TwitterBotShared.Service;

namespace JobRunner
{
    /// <summary>
    /// Simple console application that retieves Twitter both definitions from Azure Storage and 
    /// processes the requested jobs
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var twitter = new TwitterSearch();
            var botRunner = new BotRunner(twitter);
            botRunner.PostAllBotsAsync().Wait();
        }
    }
}
