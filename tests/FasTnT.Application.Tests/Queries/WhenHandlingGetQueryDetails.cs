using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingGetQueryDetails
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetQueryDetails));
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
                Parameters =
                [
                    new QueryParameter{ Name = "EQ_type", Values = ["ObjectEvent", "TestEvent"]}
                ]
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
    public void ItShouldReturnTheQueryWhenItExists()
    {
        var handler = new QueriesHandler(Context, UserContext);
        var result = handler.GetQueryDetailsAsync("QueryOne", CancellationToken.None).Result;

        Assert.IsInstanceOfType<StoredQuery>(result);

        Assert.AreEqual("QueryOne", result.Name);
        Assert.HasCount(1, result.Parameters);
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var handler = new QueriesHandler(Context, UserContext);

        await Assert.ThrowsAsync<EpcisException>(() => handler.GetQueryDetailsAsync("Unknown", CancellationToken.None));
    }
}
