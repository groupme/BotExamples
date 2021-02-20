// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace DinoBot
{
    /// <summary>
    /// Interface for the logic that determines if DinoBot should respond to a message or not
    /// </summary>
    public interface IDinoAI
    {
        /// <summary>
        /// Checks a message and determines if DinoBot should respond
        /// </summary>
        /// <param name="messageText">Text of the message to check</param>
        /// <returns>True if DinoBot should respond, false if not</returns>
        bool ShouldDinoRespond(string messageText);
    }
}
