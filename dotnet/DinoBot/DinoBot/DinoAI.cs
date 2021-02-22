// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace DinoBot
{
    /// <summary>
    /// An implementation of <see cref="IDinoAI"/> that's definitely not random
    /// </summary>
    public class DinoAI : IDinoAI
    {
        private const double ResponseWeight = 0.075;
        private Random _random;

        public DinoAI()
        {
            _random = new Random();
        }

        /// <summary>
        /// Checks a message and determines if DinoBot should respond
        /// </summary>
        /// <param name="messageText">Text of the message to check</param>
        /// <returns>True if DinoBot should respond, false if not</returns>
        public bool ShouldDinoRespond(string messageText)
        {
            if (messageText.Contains("?"))
            {
                double val = _random.NextDouble();
                if (val <= ResponseWeight)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
