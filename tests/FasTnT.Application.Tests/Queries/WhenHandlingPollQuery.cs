using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using FasTnT.Domain.Model.Queries;
using Microsoft.Extensions.Options;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingPollQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [TestMethod]
    public void ItShouldReturnAPollResponse()
    {
        var handler = new DataRetrieverHandler(Context, UserContext, Options.Create(new Constants()));
        var result = handler.QueryEventsAsync(new List<QueryParameter>(), CancellationToken.None).Result;

        Assert.IsNotNull(result);
    }
}
