using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Storage;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingDeleteQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingDeleteQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange(new object[] {
            new StoredQuery
            {
                Id = 1,
                Name = "QueryOne",
                UserId = UserContext.UserId,
                Parameters = new List<QueryParameter>
                {
                    new QueryParameter{ Name = "EQ_type", Values = new []{ "ObjectEvent", "TestEvent" }}
                }
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
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheQueryWhenItIsDeleted()
    {
        var handler = new QueriesHandler(Context, UserContext);
        var result = handler.DeleteQueryAsync("QueryOne", CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(StoredQuery));

        Assert.AreEqual("QueryOne", result.Name);
        Assert.AreEqual(1, result.Parameters.Count);
        Assert.AreEqual(0, Context.Set<StoredQuery>().Count(x => x.Name == "QueryOne"));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var handler = new QueriesHandler(Context, UserContext);
        
        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.DeleteQueryAsync("Unknown", CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryHasSubscription()
    {
        var handler = new QueriesHandler(Context, UserContext);
        
        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.DeleteQueryAsync("WithSubscription", CancellationToken.None));
        Assert.AreEqual(1, Context.Set<StoredQuery>().Count(x => x.Name == "WithSubscription"));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQUeryWasCreatedByAnotherUser()
    {
        var handler = new QueriesHandler(Context, UserContext);
        
        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.DeleteQueryAsync("FromOtherUser", CancellationToken.None));
        Assert.AreEqual(1, Context.Set<StoredQuery>().Count(x => x.Name == "FromOtherUser"));
    }
}
