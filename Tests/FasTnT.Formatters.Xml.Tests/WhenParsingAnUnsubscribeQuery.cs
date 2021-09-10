using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAnUnsubscribeQuery : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.Queries.Unsubscribe.xml";

        public object Query { get; set; }

        [TestInitialize]
        public void When()
        {
            Query = XmlQueryParser.Parse(ParseResource(ResourceName).Root);
        }

        [TestMethod]
        public void ItShouldReturnAnUnsubscribeObject()
        {
            Assert.IsInstanceOfType(Query, typeof(UnsubscribeCommand));
        }

        [TestMethod]
        public void TheUnsubscribeCommandShouldHaveTheCorrectSubscriptionId()
        {
            Assert.AreEqual("TestSubscription", (Query as UnsubscribeCommand).SubscriptionId);
        }
    }
}
