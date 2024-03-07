using FasTnT.Host.Communication.Xml.Parsers;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAGetSubscriptionIDsQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.GetSubscriptionIDs.xml";

    public object Query { get; set; }

    [TestInitialize]
    public void When()
    {
        Query = ParseResource(ResourceName).Parse();
    }

    [TestMethod]
    public void ItShouldReturnAGetSubscriptionIDsObject()
    {
        Assert.IsInstanceOfType(Query, typeof(GetSubscriptionIDs));
    }

    [TestMethod]
    public void TheGetSubscriptionIDsQueryShouldHaveTheCorrectQueryName()
    {
        Assert.AreEqual("SimpleEventQuery", (Query as GetSubscriptionIDs).QueryName);
    }
}
