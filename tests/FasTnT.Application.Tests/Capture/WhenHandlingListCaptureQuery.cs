﻿using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingListCaptureQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListCaptureQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }


    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange([
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

        Context.SaveChanges();
    }

    [TestMethod]
    public async Task ItShouldReturnTheRequests()
    {
        var handler = new CaptureHandler(Context, UserContext, new EpcisEvents(), Options.Create(new Constants()));
        var result = await handler.ListCapturesAsync(Pagination.Max, default);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task ItShouldApplyThePaginationPerPageFilter()
    {
        var handler = new CaptureHandler(Context, UserContext, new EpcisEvents(), Options.Create(new Constants()));
        var result = await handler.ListCapturesAsync(new Pagination(1, 0), default);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("001", result.First().CaptureId);
    }

    [TestMethod]
    public async Task ItShouldApplyThePaginationStartFromFilter()
    {
        var handler = new CaptureHandler(Context, UserContext, new EpcisEvents(), Options.Create(new Constants()));
        var result = await handler.ListCapturesAsync(new Pagination(int.MaxValue, 1), default);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("002", result.First().CaptureId);
    }
}
