using FasTnT.Host.Features.v1_2.Communication;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAGetSubscriptionIdsResult
{
    public GetSubscriptionIDsResult Result = new(new[] { "Sub1", "Sub2", "Sub2" });
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
        Assert.IsTrue(Formatted.Name == XName.Get("GetSubscriptionIDsResult", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.AreEqual(3, Formatted.Elements().Count());
        CollectionAssert.AreEquivalent(Result.SubscriptionIDs.ToArray(), Formatted.Elements().Select(x => x.Value).ToArray());
    }
}
