using FasTnT.Application.Handlers;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListEventTypesRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListEventTypesRequest));
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
