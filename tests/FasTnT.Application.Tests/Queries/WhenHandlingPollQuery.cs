using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingPollQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAPollResponse()
    {
        var handler = new DataRetrieverHandler(Context, UserContext);
        var result = handler.QueryEventsAsync(new List<QueryParameter>(), CancellationToken.None).Result;

        Assert.IsNotNull(result);
    }
}
