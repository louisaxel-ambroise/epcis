using FasTnT.Application.Database;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.Handlers;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingGetSubscriptionCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetSubscriptionCommand));
    readonly static TestSubscriptionListener Listener = new();

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Add(new Subscription
        {
            Name = "TestSubscription",
            QueryName = "SimpleEventQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheSubscriptionIfItExists()
    {
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);
        var result = handler.GetSubscriptionDetailsAsync("TestSubscription", CancellationToken.None).Result;

        Assert.IsInstanceOfType<Subscription>(result);
        Assert.AreEqual("SimpleEventQuery", result.QueryName);
        Assert.AreEqual("TestSubscription", result.Name);
        Assert.AreEqual("", result.Destination);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSubscriptionDoesNotExists()
    {
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);
        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.GetSubscriptionDetailsAsync("UnknownSubscription", CancellationToken.None));
    }
}
