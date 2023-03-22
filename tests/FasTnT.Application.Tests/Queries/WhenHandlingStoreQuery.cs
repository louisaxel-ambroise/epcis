using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingStoreQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingStoreQuery));
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
        Context.AddRange(new[] {
            new StoredQuery
            {
                Id = 1,
                Name = "QueryOne",
                Parameters = new List<QueryParameter>
                {
                    new QueryParameter{ Name = "EQ_type", Values = new []{ "ObjectEvent", "TestEvent" }}
                }
            }
        });
        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheQueryIfItIsSuccessfullyStored()
    {
        var handler = new QueriesHandler(Context, UserContext);
        var result = handler.StoreQueryAsync(new StoredQuery { Name = "NewQuery" }, CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(StoredQuery));

        Assert.AreEqual("NewQuery", result.Name);
        Assert.AreEqual(0, result.Parameters.Count);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfAQueryWithSameNameAlreadyExists()
    {
        var handler = new QueriesHandler(Context, UserContext);
        
        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StoreQueryAsync(new StoredQuery { Name = "QueryOne" }, CancellationToken.None));
    }
}
