using FasTnT.Domain.Queries.GetSubscriptionIds;
using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAGetSubscriptionIDsQuery : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.Queries.GetSubscriptionIDs.xml";

        public object Query { get; set; }

        [TestInitialize]
        public void When()
        {
            Query = XmlQueryParser.Parse(ParseResource(ResourceName).Root);
        }

        [TestMethod]
        public void ItShouldReturnAGetSubscriptionIDsObject()
        {
            Assert.IsInstanceOfType(Query, typeof(GetSubscriptionIdsQuery));
        }

        [TestMethod]
        public void TheGetSubscriptionIDsQueryShouldHaveTheCorrectQueryName()
        {
            Assert.AreEqual("SimpleEventQuery", (Query as GetSubscriptionIdsQuery).QueryName);
        }
    }
}
