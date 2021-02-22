// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using GroupMeShared.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DinoTests
{
    /// <summary>
    /// Tests message attachments
    /// </summary>
    [TestClass]
    public class AttachmentTests
    {
        [TestMethod]
        public void TestAttachmentHasReply()
        {
            var message = new MessageItem
            {
                Attachments = new Attachment[] {
                    new Attachment
                    {
                        Type = Attachment.ImageType
                    },
                    new Attachment
                    {
                        Type = Attachment.ReplyType
                    },
                    new Attachment
                    {
                        Type = Attachment.Emoji
                    }
                }
            };

            Attachment reply = message.GetExistingReply();
            Assert.IsNotNull(reply);
            Assert.AreEqual(Attachment.ReplyType, reply.Type);
        }

        [TestMethod]
        public void TestAttachmentNoReply()
        {
            var message = new MessageItem
            {
                Attachments = new Attachment[] {
                    new Attachment
                    {
                        Type = Attachment.ImageType
                    },
                    new Attachment
                    {
                        Type = Attachment.Emoji
                    }
                }
            };

            Attachment reply = message.GetExistingReply();
            Assert.IsNull(reply);
        }

        [TestMethod]
        public void TestAttachmentNoReplies()
        {
            var message = new MessageItem();

            Attachment reply = message.GetExistingReply();
            Assert.IsNull(reply);
        }
    }
}
