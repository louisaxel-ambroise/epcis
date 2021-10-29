using FasTnT.Application.Queries;
using FasTnT.Application.Services;
using FasTnT.Domain.Queries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetQueryNamesQuery
{
    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var queries = new IEpcisQuery[] { new SimpleEventQuery(default), new SimpleMasterDataQuery(default) };
        var handler = new GetQueryNamesQueryHandler(queries);
        var result = handler.Handle(new GetQueryNamesQuery(), default).Result;
            
        Assert.AreEqual(2, result.QueryNames.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }, result.QueryNames.ToArray());
    }
}
