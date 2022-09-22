using FasTnT.Features.v1_2.Communication.Formatters;
using FasTnT.Features.v1_2.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Features.v1_2.Tests;

[TestClass]
public class WhenFormattingAnUnsubscribeResult
{
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = XmlResponseFormatter.FormatUnsubscribeResponse();
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
