using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using Microsoft.Extensions.Options;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingPollQuery
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
    }

    [TestMethod]
    public void ItShouldReturnAPollResponse()
    {
        var handler = new DataRetrieverHandler(Context.DbContext, UserContext, Options.Create(new Constants()));
        var result = handler.QueryEventsAsync([], CancellationToken.None).Result;

        Assert.IsNotNull(result);
    }
}
