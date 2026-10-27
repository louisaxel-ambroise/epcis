using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingListQueries
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListQueries));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.DbContext.AddRange([
            new StoredQuery
            {
                Id = 1,
                Name = "QueryOne"
            },
            new StoredQuery
            {
                Id = 2,
                Name = "QueryTwo"
            }
        ]);
        Context.DbContext.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var handler = new QueriesHandler(Context.DbContext, UserContext);
        var result = handler.ListQueriesAsync(Pagination.Max, CancellationToken.None).Result;

        Assert.IsInstanceOfType<List<StoredQuery>>(result);

        Assert.HasCount(2, result);
        Assert.AreSequenceEqual(["QueryOne", "QueryTwo"], [.. result.Select(x => x.Name)], SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void ItShouldApplyThePaginationPerPageFilter()
    {
        var handler = new QueriesHandler(Context.DbContext, UserContext);
        var result = handler.ListQueriesAsync(new Pagination(1, 0), CancellationToken.None).Result;

        Assert.IsInstanceOfType<List<StoredQuery>>(result);
        Assert.HasCount(1, result);
        Assert.AreSequenceEqual(["QueryOne"], [.. result.Select(x => x.Name)], SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void ItShouldApplyThePaginationStartFromFilter()
    {
        var handler = new QueriesHandler(Context.DbContext, UserContext);
        var result = handler.ListQueriesAsync(new Pagination(30, 1), CancellationToken.None).Result;

        Assert.IsInstanceOfType<List<StoredQuery>>(result);
        Assert.HasCount(1, result);
        Assert.AreSequenceEqual(["QueryTwo"], [.. result.Select(x => x.Name)], SequenceOrder.InAnyOrder);
    }
}
