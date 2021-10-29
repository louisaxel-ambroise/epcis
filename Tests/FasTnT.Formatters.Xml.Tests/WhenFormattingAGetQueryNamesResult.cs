using FasTnT.Domain.Queries.GetQueryNames;
using FasTnT.Formatter.Xml.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenFormattingAGetQueryNamesResult
    {
        public GetQueryNamesResult Result = new(new[] { "QueryOne", "QueryTwo" });
        public XElement Formatted { get; set; }

        [TestInitialize]
        public void When()
        {
            Formatted = XmlResponseFormatter.FormatGetQueryNames(Result);
        }

        [TestMethod]
        public void ItShouldReturnAnXElement()
        {
            Assert.IsNotNull(Formatted);
        }

        [TestMethod]
        public void TheXmlShouldBeCorrectlyFormatter()
        {
            Assert.IsTrue(Formatted.Name == XName.Get("GetQueryNamesResult", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.AreEqual(2, Formatted.Elements().Count());
            CollectionAssert.AreEquivalent(Result.QueryNames.ToArray(), Formatted.Elements().Select(x => x.Value).ToArray());
        }
    }
}
