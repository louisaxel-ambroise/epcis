using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Formatter.Xml.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenFormattingAnUnsubscribeResult
    {
        public UnsubscribeResult Result = new();
        public XElement Formatted { get; set; }

        [TestInitialize]
        public void When()
        {
            Formatted = XmlResponseFormatter.FormatUnsubscribeResponse(Result);
        }

        [TestMethod]
        public void ItShouldReturnAnXElement()
        {
            Assert.IsNotNull(Formatted);
        }

        [TestMethod]
        public void TheXmlShouldBeCorrectlyFormatter()
        {
            Assert.IsTrue(Formatted.Name == XName.Get("UnsubscribeResult", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.IsTrue(Formatted.IsEmpty);
        }
    }
}
