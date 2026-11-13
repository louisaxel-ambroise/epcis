using FasTnT.Application.Handlers;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using Microsoft.Extensions.Options;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingGetCaptureDetailQuery
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetCaptureDetailQuery));
    readonly static TestCurrentUser UserContext = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.DbContext.AddRange([
            new Request
            {
                Id = 1,
                UserId = UserContext.UserId,
                CaptureId = "001",
                SchemaVersion = "2.0",
                RecordTime = DateTime.UtcNow,
                DocumentTime = DateTime.UtcNow,
                Events = [new Event { Type = EventType.ObjectEvent }]
            },
            new Request
            {
                Id = 2,
                UserId = UserContext.UserId,
                CaptureId = "002",
                SchemaVersion = "2.0",
                RecordTime = DateTime.UtcNow,
                DocumentTime = DateTime.UtcNow,
                Events = [new Event { Type = EventType.ObjectEvent }]
            }
        ]);

        Context.DbContext.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheRequests()
    {
        var handler = new CaptureHandler(Context.DbContext, UserContext, Options.Create(new Constants()));
        var result = handler.GetCaptureDetailsAsync("001", default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual("001", result.CaptureId);
        Assert.AreEqual("2.0", result.SchemaVersion);
        Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheCaptureDoesNotExist()
    {
        var handler = new CaptureHandler(Context.DbContext, UserContext, Options.Create(new Constants()));

        Assert.ThrowsAsync<EpcisException>(() => handler.GetCaptureDetailsAsync("unknown", default));
    }
}
