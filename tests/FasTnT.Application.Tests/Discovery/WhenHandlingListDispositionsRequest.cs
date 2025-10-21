using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListDispositionsRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListDispositionsRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public void Initialize()
    {
        Context.Add(new Domain.Model.Request
        {
            RecordTime = DateTime.Now,
            DocumentTime = DateTime.Now,
            SchemaVersion = "2.0",
            UserId = "TESTUSER",
            Events =
            [
                new Event
                {
                    Disposition = "D1"
                },
                new Event
                {
                    Disposition = "D2"
                }
            ]
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheDispositionsIfPageSizeIsGreaterThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(10, 0);

        var result = handler.ListDispositions(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheRequestedNumberOfDispositionsIfPageSizeIsLowerThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(1, 0);

        var result = handler.ListDispositions(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheCorrectPageOfData()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(10, 1);

        var result = handler.ListDispositions(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }
}
