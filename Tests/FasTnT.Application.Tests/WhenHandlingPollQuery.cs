using FasTnT.Application.Queries;
using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using FasTnT.Infrastructure.Database;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingPollQuery
{
    readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(Tests.WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var queries = new IEpcisQuery[] { new SimpleEventQuery(Context), new SimpleMasterDataQuery(Context) };
        var handler = new PollQueryHandler(queries, UserContext);
        var request = new PollQuery("SimpleEventQuery", new List<QueryParameter>());
        var result = handler.Handle(request, default).Result;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var queries = new IEpcisQuery[] { new SimpleEventQuery(Context), new SimpleMasterDataQuery(Context) };
        var handler = new PollQueryHandler(queries, UserContext);
        var request = new PollQuery("UnknownQuery", new List<QueryParameter>());

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.Handle(request, default));
    }
}
