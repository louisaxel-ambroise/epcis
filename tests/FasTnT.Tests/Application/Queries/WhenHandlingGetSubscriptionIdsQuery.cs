using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Handlers;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingGetSubscriptionIdsQuery
{
    public readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetSubscriptionIdsQuery));

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Set<Subscription>().Add(new Subscription
        {
            Name = "SubscriptionTest",
            QueryName = "TestQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });
        Context.Set<Subscription>().Add(new Subscription
        {
            Name = "OtherSubscription",
            QueryName = "OtherQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheListOfExistingSubscriptionIdsForTheSpecifiedRequest()
    {
        var handler = new SubscriptionsHandler(Context, default);
        var result = handler.ListSubscriptionsAsync("TestQuery", CancellationToken.None).Result;

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("SubscriptionTest", result.First().Name);
    }

    [TestMethod]
    public void ItShouldReturnAnEmptyListWhenNoSubscriptionMatch()
    {
        var handler = new SubscriptionsHandler(Context, default);
        var result = handler.ListSubscriptionsAsync("UnknownQuery", CancellationToken.None).Result;

        Assert.AreEqual(0, result.Count());
    }
}
