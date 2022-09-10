using FasTnT.Domain.Queries.GetSubscriptionIds;
using FasTnT.Features.v1_2.Communication.Parsers;

namespace FasTnT.Features.v1_2.Tests;

[TestClass]
public class WhenParsingAGetSubscriptionIDsQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v1_2.Tests.Resources.Queries.GetSubscriptionIDs.xml";

    public object Query { get; set; }

    [TestInitialize]
    public void When()
    {
        Query = XmlQueryParser.Parse(ParseResource(ResourceName).Root);
    }

    [TestMethod]
    public void ItShouldReturnAGetSubscriptionIDsObject()
    {
        Assert.IsInstanceOfType(Query, typeof(GetSubscriptionIdsQuery));
    }

    [TestMethod]
    public void TheGetSubscriptionIDsQueryShouldHaveTheCorrectQueryName()
    {
        Assert.AreEqual("SimpleEventQuery", (Query as GetSubscriptionIdsQuery).QueryName);
    }
}
