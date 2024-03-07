using FasTnT.Host.Communication.Xml.Formatters;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAGetQueryNamesResult
{
    public GetQueryNamesResult Result = new(new[] { "QueryOne", "QueryTwo" });
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
        Assert.IsTrue(Formatted.Name == XName.Get("GetQueryNamesResult", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.AreEqual(2, Formatted.Elements().Count());
        CollectionAssert.AreEquivalent(Result.QueryNames.ToArray(), Formatted.Elements().Select(x => x.Value).ToArray());
    }
}
