using FasTnT.Application.Handlers;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingGetSubscriptionIdsQuery
{
    public readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetSubscriptionIdsQuery));

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.DbContext.Set<Subscription>().Add(new Subscription
        {
            Name = "SubscriptionTest",
            QueryName = "TestQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });
        Context.DbContext.Set<Subscription>().Add(new Subscription
        {
            Name = "OtherSubscription",
            QueryName = "OtherQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });

        Context.DbContext.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheListOfExistingSubscriptionIdsForTheSpecifiedRequest()
    {
        var handler = new SubscriptionsHandler(Context.DbContext, default);
        var result = handler.ListSubscriptionsAsync("TestQuery", CancellationToken.None).Result;

        Assert.HasCount(1, result);
        Assert.AreEqual("SubscriptionTest", result.First().Name);
    }

    [TestMethod]
    public void ItShouldReturnAnEmptyListWhenNoSubscriptionMatch()
    {
        var handler = new SubscriptionsHandler(Context.DbContext, default);
        var result = handler.ListSubscriptionsAsync("UnknownQuery", CancellationToken.None).Result;

        Assert.HasCount(0, result);
    }
}
