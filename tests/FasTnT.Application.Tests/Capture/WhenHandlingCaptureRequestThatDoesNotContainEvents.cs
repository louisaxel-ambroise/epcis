﻿using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequestThatDoesNotContainEvents
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [TestMethod]
    public async Task ItShoultThrowAnException()
    {
        var handler = new CaptureHandler(Context, UserContext, new EpcisEvents(), Options.Create(new Constants()));
        var request = new Request { SchemaVersion = "1.0" };

        await Assert.ThrowsAsync<EpcisException>(() => handler.StoreAsync(request, default));
    }
}
