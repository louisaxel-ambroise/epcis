﻿using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingListQueries
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListQueries));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange([
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
        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var handler = new QueriesHandler(Context, UserContext);
        var result = handler.ListQueriesAsync(Pagination.Max, CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(2, result.Count());
        CollectionAssert.AreEquivalent(new[] { "QueryOne", "QueryTwo" }, result.Select(x => x.Name).ToArray());
    }

    [TestMethod]
    public void ItShouldApplyThePaginationPerPageFilter()
    {
        var handler = new QueriesHandler(Context, UserContext);
        var result = handler.ListQueriesAsync(new Pagination(1, 0), CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(1, result.Count());
        CollectionAssert.AreEquivalent(new[] { "QueryOne" }, result.Select(x => x.Name).ToArray());
    }

    [TestMethod]
    public void ItShouldApplyThePaginationStartFromFilter()
    {
        var handler = new QueriesHandler(Context, UserContext);
        var result = handler.ListQueriesAsync(new Pagination(30, 1), CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(1, result.Count());
        CollectionAssert.AreEquivalent(new[] { "QueryTwo" }, result.Select(x => x.Name).ToArray());
    }
}
