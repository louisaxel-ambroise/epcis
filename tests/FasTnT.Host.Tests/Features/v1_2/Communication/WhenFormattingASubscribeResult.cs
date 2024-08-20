using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingASubscribeResult
{
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = SoapResponseFormatter.Format(new Subscription());
    }

    [TestMethod]
    public void ItShouldReturnAnXElement()
    {
        Assert.IsNotNull(Formatted);
    }

    [TestMethod]
    public void TheXmlShouldBeCorrectlyFormatter()
    {
        Assert.IsTrue(Formatted.Name == XName.Get("SubscribeResult", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.IsTrue(Formatted.IsEmpty);
    }
}
