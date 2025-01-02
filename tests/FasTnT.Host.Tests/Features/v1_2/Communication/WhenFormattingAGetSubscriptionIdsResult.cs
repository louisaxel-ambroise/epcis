using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Communication.Xml.Formatters;
using FasTnT.Host.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAGetSubscriptionIdsResult
{
    public ListSubscriptionsResult Result = new([new Subscription { Name = "Sub1" }, new Subscription { Name = "Sub2" }, new Subscription { Name = "Sub2" }]);
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
        Assert.IsTrue(Formatted.Name == XName.Get("GetSubscriptionIDsResult", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.AreEqual(3, Formatted.Elements().Count());
        CollectionAssert.AreEquivalent(Result.Subscriptions.Select(x => x.Name).ToArray(), Formatted.Elements().Select(x => x.Value).ToArray());
    }
}
