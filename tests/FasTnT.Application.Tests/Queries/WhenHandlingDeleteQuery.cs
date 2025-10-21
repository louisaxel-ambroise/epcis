using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingDeleteQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingDeleteQuery));
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

        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    [TestMethod]
    public void ItShouldReturnTheQueryWhenItIsDeleted()
    {
        var handler = new QueriesHandler(Context, UserContext);
        var result = handler.DeleteQueryAsync("QueryOne", CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(StoredQuery));

        Assert.AreEqual("QueryOne", result.Name);
        Assert.HasCount(1, result.Parameters);
        Assert.AreEqual(0, Context.Set<StoredQuery>().Count(x => x.Name == "QueryOne"));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var handler = new QueriesHandler(Context, UserContext);

        await Assert.ThrowsAsync<EpcisException>(() => handler.DeleteQueryAsync("Unknown", CancellationToken.None));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheQueryHasSubscription()
    {
        var handler = new QueriesHandler(Context, UserContext);

        await Assert.ThrowsAsync<EpcisException>(() => handler.DeleteQueryAsync("WithSubscription", CancellationToken.None));
        Assert.AreEqual(1, Context.Set<StoredQuery>().Count(x => x.Name == "WithSubscription"));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheQUeryWasCreatedByAnotherUser()
    {
        var handler = new QueriesHandler(Context, UserContext);

        await Assert.ThrowsAsync<EpcisException>(() => handler.DeleteQueryAsync("FromOtherUser", CancellationToken.None));
        Assert.AreEqual(1, Context.Set<StoredQuery>().Count(x => x.Name == "FromOtherUser"));
    }
}
