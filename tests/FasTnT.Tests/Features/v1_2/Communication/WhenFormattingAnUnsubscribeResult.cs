using FasTnT.Host.Features.v1_2.Communication;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAnUnsubscribeResult
{
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = XmlResponseFormatter.Format(new UnsubscribeResult());
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
