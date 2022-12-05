using FasTnT.Application.EfCore.UseCases.TopLevelResources;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.EfCore.Store;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListDispositionsRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListDispositionsRequest));

    [TestInitialize]
    public void Initialize()
    {
        Context.Requests.Add(new Domain.Model.Request
        {
            CaptureDate = DateTimeOffset.Now,
            DocumentTime = DateTimeOffset.Now,
            SchemaVersion = "2.0",
            UserId = "TESTUSER",
            Events = new List<Event>
            {
                new Event
                {
                    Disposition = "D1"
                },
                new Event
                {
                    Disposition = "D2"
                }
            }
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheDispositionsIfPageSizeIsGreaterThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceUseCasesHandler(Context);
        var request = new Pagination(10, 0);

        var result = handler.ListDispositions(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheRequestedNumberOfDispositionsIfPageSizeIsLowerThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceUseCasesHandler(Context);
        var request = new Pagination(1, 0);

        var result = handler.ListDispositions(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheCorrectPageOfData()
    {
        var handler = new TopLevelResourceUseCasesHandler(Context);
        var request = new Pagination(10, 1);

        var result = handler.ListDispositions(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }
}
