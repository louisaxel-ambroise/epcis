using FasTnT.Application.Handlers;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingPollQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [TestMethod]
    public void ItShouldReturnAPollResponse()
    {
        var handler = new DataRetrieverHandler(Context, UserContext);
        var result = handler.QueryEventsAsync(new List<QueryParameter>(), CancellationToken.None).Result;

        Assert.IsNotNull(result);
    }
}
