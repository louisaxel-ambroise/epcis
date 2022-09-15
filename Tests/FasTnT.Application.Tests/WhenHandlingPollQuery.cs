using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.UseCases.StandardQueries;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingPollQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAPollResponse()
    {
        var queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        var handler = new StandardQueriesUseCasesHandler(Context, UserContext, queries);
        var result = handler.ExecuteQueryAsync("SimpleEventQuery", new List<QueryParameter>(), CancellationToken.None).Result;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        var handler = new StandardQueriesUseCasesHandler(Context, UserContext, queries);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.ExecuteQueryAsync("UnknownQuery", new List<QueryParameter>(), CancellationToken.None));
    }
}
