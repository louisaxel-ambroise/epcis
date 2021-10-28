using FasTnT.Domain.Queries.GetVendorVersion;
using FasTnT.Formatter.Xml.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenFormattingAGetVendorVersionResult
    {
        public GetVendorVersionResult Result = new("1.1");
        public XElement Formatted { get; set; }

        [TestInitialize]
        public void When()
        {
            Formatted = XmlResponseFormatter.FormatVendorVersion(Result);
        }

        [TestMethod]
        public void ItShouldReturnAnXElement()
        {
            Assert.IsNotNull(Formatted);
        }

        [TestMethod]
        public void TheXmlShouldBeCorrectlyFormatter()
        {
            Assert.IsTrue(Formatted.Name == XName.Get("GetVendorVersionResult", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.IsTrue(Formatted.Value == Result.Version);
        }
    }
}
