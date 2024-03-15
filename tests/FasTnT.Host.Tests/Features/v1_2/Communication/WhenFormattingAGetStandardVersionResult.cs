using FasTnT.Host.Features.v1_2.Communication.Formatters;
using FasTnT.Host.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAGetStandardVersionResult
{
    public GetStandardVersionResult Result = new("1.2");
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = XmlResponseFormatter.Format(Result);
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
