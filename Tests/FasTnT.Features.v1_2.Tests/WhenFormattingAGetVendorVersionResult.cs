using FasTnT.Features.v1_2.Communication.Formatters;
using FasTnT.Features.v1_2.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Features.v1_2.Tests;

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
