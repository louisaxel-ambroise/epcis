using FasTnT.Domain.Exceptions;
using FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v2_0.Communication;

[TestClass]
public class WhenFormattingAnErrorResponse
{
    public EpcisException Result = new(ExceptionType.NoSuchNameException, "Query does not exist");
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
        Assert.IsTrue(Formatted.Name == XName.Get("problem", "urn:ietf:rfc:7807"));
        Assert.AreEqual(3, Formatted.Elements().Count());
        Assert.AreEqual("epcisException:NoSuchNameException", Formatted.Element(XName.Get("type", "urn:ietf:rfc:7807")).Value);
        Assert.AreEqual(Result.Message, Formatted.Element(XName.Get("title", "urn:ietf:rfc:7807")).Value);
        Assert.AreEqual("404", Formatted.Element(XName.Get("status", "urn:ietf:rfc:7807")).Value);
    }

    [TestMethod]
    public void ThereShouldNotBeASubscriptionIDField()
    {
        Assert.IsNull(Formatted.Element("subscriptionID"));
    }

    [TestMethod]
    public void ThereShouldNotBeAQueryNameField()
    {
        Assert.IsNull(Formatted.Element("queryName"));
    }
}
