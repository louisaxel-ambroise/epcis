using FasTnT.Application.Queries;
using FasTnT.Domain.Queries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetQueryNamesQuery
{
    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var queries = new Services.IEpcisQuery[] { new SimpleEventQuery(default), new SimpleMasterDataQuery(default) };
        var handler = new GetQueryNamesQueryHandler(queries);
        var result = handler.Handle(new GetQueryNamesQuery(), default).Result;

        Assert.IsInstanceOfType(result, typeof(GetQueryNamesResult));

        var queryNames = (GetQueryNamesResult)result;
        Assert.AreEqual(2, queryNames.QueryNames.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }, queryNames.QueryNames.ToArray());
    }
}
