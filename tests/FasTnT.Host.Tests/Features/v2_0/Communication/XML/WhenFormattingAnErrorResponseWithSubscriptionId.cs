using FasTnT.Application.Domain.Exceptions;
using FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingAnErrorResponseWithSubscriptionId
{
    public EpcisException Result = new(ExceptionType.NoSuchNameException, "Query does not exist") { SubscriptionId = "TestSubscription" };
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = XmlResponseFormatter.FormatError(Result);
    }

    [TestMethod]
    public void ItShouldReturnAnXElement()
    {
        Assert.IsNotNull(Formatted);
    }

    [TestMethod]
    public void TheXmlShouldBeCorrectlyFormatted()
    {
        Assert.IsTrue(Formatted.Name == XName.Get("NoSuchNameException", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.AreEqual(2, Formatted.Elements().Count());
        Assert.AreEqual(Result.Message, Formatted.Element("reason").Value);
        Assert.AreEqual(Result.SubscriptionId, Formatted.Element("subscriptionID").Value);
    }
    [TestMethod]
    public void ThereShouldNotBeAQueryNameField()
    {
        Assert.IsNull(Formatted.Element("queryName"));
    }
}
