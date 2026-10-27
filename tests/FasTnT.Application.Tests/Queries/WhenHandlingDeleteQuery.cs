using FasTnT.Application.Handlers;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingDeleteQuery
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingDeleteQuery));
    readonly static TestCurrentUser UserContext = new();

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
                Name = "QueryOne",
                UserId = UserContext.UserId,
                Parameters =
                [
                    new QueryParameter{ Name = "EQ_type", Values = ["ObjectEvent", "TestEvent"]}
                ]
            },
            new StoredQuery
            {
                Id = 2,
                Name = "WithSubscription",
                UserId = UserContext.UserId
            },
            new StoredQuery
            {
                Id = 3,
                Name = "FromOtherUser",
                UserId = "1234"
            },
            new Subscription { Id = 1, QueryName = "WithSubscription", Name = "test", Destination = "", FormatterName = "" }
        ]);

        Context.DbContext.SaveChanges();
        Context.DbContext.ChangeTracker.Clear();
    }

    [TestMethod]
    public void ItShouldReturnTheQueryWhenItIsDeleted()
    {
        var handler = new QueriesHandler(Context.DbContext, UserContext);
        var result = handler.DeleteQueryAsync("QueryOne", CancellationToken.None).Result;

        Assert.IsInstanceOfType<StoredQuery>(result);

        Assert.AreEqual("QueryOne", result.Name);
        Assert.HasCount(1, result.Parameters);
        Assert.AreEqual(0, Context.DbContext.Set<StoredQuery>().Count(x => x.Name == "QueryOne"));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var handler = new QueriesHandler(Context.DbContext, UserContext);

        Assert.ThrowsAsync<EpcisException>(() => handler.DeleteQueryAsync("Unknown", CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryHasSubscription()
    {
        var handler = new QueriesHandler(Context.DbContext, UserContext);

        Assert.ThrowsAsync<EpcisException>(() => handler.DeleteQueryAsync("WithSubscription", CancellationToken.None));
        Assert.AreEqual(1, Context.DbContext.Set<StoredQuery>().Count(x => x.Name == "WithSubscription"));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQUeryWasCreatedByAnotherUser()
    {
        var handler = new QueriesHandler(Context.DbContext, UserContext);

        Assert.ThrowsAsync<EpcisException>(() => handler.DeleteQueryAsync("FromOtherUser", CancellationToken.None));
        Assert.AreEqual(1, Context.DbContext.Set<StoredQuery>().Count(x => x.Name == "FromOtherUser"));
    }
}
