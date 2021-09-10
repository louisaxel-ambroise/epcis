using FasTnT.Domain.Queries.GetQueryNames;
using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAGetQueryNamesQuery : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.Queries.GetQueryNames.xml";

        public object Query { get; set; }

        [TestInitialize]
        public void When()
        {
            Query = XmlQueryParser.Parse(ParseResource(ResourceName).Root);
        }

        [TestMethod]
        public void ItShouldReturnAGetQueryNamesObject()
        {
            Assert.IsInstanceOfType(Query, typeof(GetQueryNamesQuery));
        }
    }
}
