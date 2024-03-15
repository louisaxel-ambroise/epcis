using FasTnT.Host.Features.v1_2.Communication.Parsers;
using FasTnT.Host.Interfaces;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAnUnsubscribeQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.Unsubscribe.xml";

    public object Query { get; set; }

    [TestInitialize]
    public void When()
    {
        Query = XmlQueryParser.Parse(ParseResource(ResourceName).Root);
    }

    [TestMethod]
    public void ItShouldReturnAnUnsubscribeObject()
    {
        Assert.IsInstanceOfType(Query, typeof(Unsubscribe));
    }

    [TestMethod]
    public void TheUnsubscribeCommandShouldHaveTheCorrectSubscriptionId()
    {
        Assert.AreEqual("TestSubscription", (Query as Unsubscribe).SubscriptionId);
    }
}
