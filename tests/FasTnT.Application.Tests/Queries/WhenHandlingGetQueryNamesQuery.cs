using FasTnT.Application.Relational;
using FasTnT.Application.Relational.Services.Queries;
using FasTnT.Application.Relational.UseCases.Queries;
using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingGetQueryNamesQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetQueryNamesQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var queries = new IEpcisDataSource[] { new SimpleEventQuery(Context), new SimpleMasterDataQuery(Context) };
        var handler = new QueriesUseCasesHandler(Context, UserContext, queries);
        var result = handler.ListQueriesAsync(Pagination.Max, CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(2, result.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }, result.Select(x => x.Name).ToArray());
    }

    [TestMethod]
    public void ItShouldApplyThePaginationPerPageFilter()
    {
        var queries = new IEpcisDataSource[] { new SimpleEventQuery(Context), new SimpleMasterDataQuery(Context) };
        var handler = new QueriesUseCasesHandler(Context, UserContext, queries);
        var result = handler.ListQueriesAsync(new Pagination(1, 0), CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(1, result.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleEventQuery"  }, result.Select(x => x.Name).ToArray());
    }

    [TestMethod]
    public void ItShouldApplyThePaginationStartFromFilter()
    {
        var queries = new IEpcisDataSource[] { new SimpleEventQuery(Context), new SimpleMasterDataQuery(Context) };
        var handler = new QueriesUseCasesHandler(Context, UserContext, queries);
        var result = handler.ListQueriesAsync(new Pagination(30, 1), CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(1, result.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleMasterDataQuery" }, result.Select(x => x.Name).ToArray());
    }
}
