// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DinoBot;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DinoTests
{
    [TestClass]
    public class DinoAiTests
    {
        [TestMethod]
        public void TestDinoAiNoQuestion()
        {
            var ai = new DinoAI();
            bool shouldPost = ai.ShouldDinoRespond("hi");
            Assert.IsFalse(shouldPost);
        }
    }
}
