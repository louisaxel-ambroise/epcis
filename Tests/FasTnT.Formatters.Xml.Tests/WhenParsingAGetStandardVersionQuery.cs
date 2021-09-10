using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAGetStandardVersionQuery : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.Queries.GetStandardVersion.xml";

        public object Query { get; set; }

        [TestInitialize]
        public void When()
        {
            Query = XmlQueryParser.Parse(ParseResource(ResourceName).Root);
        }

        [TestMethod]
        public void ItShouldReturnAGetStandardVersionObject()
        {
            Assert.IsInstanceOfType(Query, typeof(GetStandardVersionQuery));
        }
    }
}
