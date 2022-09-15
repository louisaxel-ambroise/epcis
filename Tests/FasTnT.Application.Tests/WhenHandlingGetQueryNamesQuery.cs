using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.UseCases.StandardQueries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetQueryNamesQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingPollQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        var handler = new StandardQueriesUseCasesHandler(Context, UserContext, queries);
        var result = handler.GetQueryNamesAsync(CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(IEnumerable<string>));

        Assert.AreEqual(2, result.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }, result.ToArray());
    }
}
