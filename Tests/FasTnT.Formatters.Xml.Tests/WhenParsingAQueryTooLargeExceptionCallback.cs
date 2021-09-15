using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAQueryTooLargeExceptionCallback : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.CallbackRequests.QueryTooLargeRequest.xml";

        public Request Request { get; set; }

        [TestInitialize]
        public void When()
        {
            Request = XmlEpcisDocumentParser.Parse(ParseResource(ResourceName).Root);
        }

        [TestMethod]
        public void RequestShouldNotSpecifyUser()
        {
            Assert.IsNull(Request.User);
            Assert.AreEqual(default, Request.UserId);
        }

        [TestMethod]
        public void RequestShouldContainASubscriptionCallback()
        {
            Assert.IsNotNull(Request.SubscriptionCallback, "Request should contain a subscription callback");
        }

        [TestMethod]
        public void RequestShouldNotContainEventOrMasterdata()
        {
            Assert.IsTrue(Request.Events.Count == 0, "Request should not contain any event");
            Assert.IsTrue(Request.Masterdata.Count == 0, "Request should not contain any masterdata");
        }

        [TestMethod]
        public void SubscriptionCallbackTypeShouldBeQueryTooLargeException()
        {
            Assert.AreEqual(QueryCallbackType.QueryTooLargeException, Request.SubscriptionCallback.CallbackType);
        }

        [TestMethod]
        public void SubscriptionCallbackReasonShouldMatchTheRequestValue()
        {
            Assert.AreEqual("This is a reason", Request.SubscriptionCallback.Reason);
        }

        [TestMethod]
        public void SubscriptionIdShouldMatchTheRequestValue()
        {
            Assert.AreEqual("TestSubscription1", Request.SubscriptionCallback.SubscriptionId);
        }
    }
}
