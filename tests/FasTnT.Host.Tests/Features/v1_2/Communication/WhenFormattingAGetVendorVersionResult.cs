using FasTnT.Host.Communication.Xml.Formatters;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAGetVendorVersionResult
{
    public GetVendorVersionResult Result = new("1.1");
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = SoapResponseFormatter.Format(Result);
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
