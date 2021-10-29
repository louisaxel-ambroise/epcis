using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAnInvalidQuery : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.Queries.InvalidQuery.xml";

        [TestMethod]
        public void ItShouldReturnAGetQueryNamesObject()
        {
            Assert.ThrowsException<AggregateException>(() => XmlQueryParser.Parse(ParseResource(ResourceName).Root));
        }
    }
}
