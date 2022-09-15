using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.UseCases.ExecuteStandardQuery;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingPollQuery
{
    readonly static EpcisContext Context = Tests.Context.EpcisTestContext.GetContext(nameof(Tests.WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAPollResponse()
    {
        var queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        var handler = new ExecuteStandardQueryHandler(Context, UserContext, queries);
        var result = handler.ExecuteQueryAsync("SimpleEventQuery", new List<QueryParameter>(), CancellationToken.None).Result;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        var handler = new ExecuteStandardQueryHandler(Context, UserContext, queries);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.ExecuteQueryAsync("UnknownQuery", new List<QueryParameter>(), CancellationToken.None));
    }
}
