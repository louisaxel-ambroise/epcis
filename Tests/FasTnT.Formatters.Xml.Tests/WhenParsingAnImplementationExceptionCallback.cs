using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAnImplementationExceptionCallback : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.CallbackRequests.ImplementationExceptionRequest.xml";

        public Request Request { get; set; }

        [TestInitialize]
        public void When()
        {
            Request = XmlEpcisDocumentParser.Parse(ParseResource(ResourceName).Root);
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
            Assert.AreEqual(QueryCallbackType.ImplementationException, Request.SubscriptionCallback.CallbackType);
        }

        [TestMethod]
        public void SubscriptionCallbackReasonShouldMatchTheRequestValue()
        {
            Assert.AreEqual("Query parameter not supported", Request.SubscriptionCallback.Reason);
        }

        [TestMethod]
        public void SubscriptionIdShouldMatchTheRequestValue()
        {
            Assert.AreEqual("TestID", Request.SubscriptionCallback.SubscriptionId);
        }
    }
}
