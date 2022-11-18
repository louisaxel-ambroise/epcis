using FasTnT.Application.EfCore.Store;
using FasTnT.Application.EfCore.UseCases.Subscriptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingGetSubscriptionIdsQuery
{
    public readonly static EpcisContext Context = Tests.Context.EpcisTestContext.GetContext(nameof(WhenHandlingGetSubscriptionIdsQuery));

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Subscriptions.Add(new Subscription
        {
            Name = "SubscriptionTest",
            QueryName = "TestQuery",
            FormatterName = "TestFormatter"
        });
        Context.Subscriptions.Add(new Subscription
        {
            Name = "OtherSubscription",
            QueryName = "OtherQuery",
            FormatterName = "TestFormatter"
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheListOfExistingSubscriptionIdsForTheSpecifiedRequest()
    {
        var handler = new SubscriptionsUseCasesHandler(Context, default, default);
        var result = handler.ListSubscriptionsAsync("TestQuery", CancellationToken.None).Result;

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("SubscriptionTest", result.First().Name);
    }

    [TestMethod]
    public void ItShouldReturnAnEmptyListWhenNoSubscriptionMatch()
    {
        var handler = new SubscriptionsUseCasesHandler(Context, default, default);
        var result = handler.ListSubscriptionsAsync("UnknownQuery", CancellationToken.None).Result;

        Assert.AreEqual(0, result.Count());
    }
}
