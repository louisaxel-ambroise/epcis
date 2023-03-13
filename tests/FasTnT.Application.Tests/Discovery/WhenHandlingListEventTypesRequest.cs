using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListEventTypesRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListEventTypesRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnAllTheEventTypes()
    {
        var request = new Pagination(10, 0);
        var result = TopLevelResourceHandler.ListEventTypes(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheCorrectPageOfData()
    {
        var request = new Pagination(10, 1);
        var result = TopLevelResourceHandler.ListEventTypes(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.Count());
    }
}
