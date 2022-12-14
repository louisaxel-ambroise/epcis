using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Domain.Exceptions;
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
        var handler = new QueriesUseCasesHandler(Context, UserContext);
        var result = handler.ExecuteQueryAsync("SimpleEventQuery", new List<QueryParameter>(), CancellationToken.None).Result;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
    {
        var handler = new QueriesUseCasesHandler(Context, UserContext);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.ExecuteQueryAsync("UnknownQuery", new List<QueryParameter>(), CancellationToken.None));
    }
}
