using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Formatter.Xml.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenFormattingAGetStandardVersionResult
    {
        public GetStandardVersionResult Result = new("1.2");
        public XElement Formatted { get; set; }

        [TestInitialize]
        public void When()
        {
            Formatted = XmlResponseFormatter.FormatStandardVersion(Result);
        }

        [TestMethod]
        public void ItShouldReturnAnXElement()
        {
            Assert.IsNotNull(Formatted);
        }

        [TestMethod]
        public void TheXmlShouldBeCorrectlyFormatter()
        {
            Assert.IsTrue(Formatted.Name == XName.Get("GetStandardVersionResult", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.IsTrue(Formatted.Value == Result.Version);
        }
    }
}
