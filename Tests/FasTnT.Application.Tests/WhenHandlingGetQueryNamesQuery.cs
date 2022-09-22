using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetQueryNamesQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetQueryNamesQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var queries = new IEpcisDataSource[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        var handler = new QueriesUseCasesHandler(Context, UserContext, queries);
        var result = handler.ListQueriesAsync(CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(2, result.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }, result.Select(x => x.Name).ToArray());
    }
}
