﻿using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.Handlers;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Exceptions;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequestGivenTheStandardBusinessHeaderInInvalid
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequestGivenTheStandardBusinessHeaderInInvalid));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static TestSubscriptionListener SubscriptionListener = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionAnNotCaptureTheRequest()
    {
        var handler = new CaptureHandler(Context, UserContext, SubscriptionListener);
        var request = new Request 
        { 
            SchemaVersion = "1.0", 
            StandardBusinessHeader = new StandardBusinessHeader(),
            Events = new() { new Event { Type = EventType.ObjectEvent } } 
        };
        
        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StoreAsync(request, default));
        Assert.AreEqual(0, Context.Set<Request>().Count());
        Assert.IsFalse(SubscriptionListener.IsTriggered("stream"));
    }
}