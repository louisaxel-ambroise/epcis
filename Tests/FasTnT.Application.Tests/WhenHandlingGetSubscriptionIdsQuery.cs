using FasTnT.Application.Queries.GetSubscriptionIds;
using FasTnT.Application.Store;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Queries.GetSubscriptionIds;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetSubscriptionIdsQuery
{
    public readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(Tests.WhenHandlingGetSubscriptionIdsQuery));

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Subscriptions.Add(new Subscription
        {
            Name = "SubscriptionTest",
            QueryName = "TestQuery"
        });
        Context.Subscriptions.Add(new Subscription
        {
            Name = "OtherSubscription",
            QueryName = "OtherQuery"
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheListOfExistingSubscriptionIdsForTheSpecifiedRequest()
    {
        var handler = new GetSubscriptionIdsQueryHandler(Context);
        var result = handler.Handle(new GetSubscriptionIdsQuery { QueryName = "TestQuery" }, default).Result;
            
        Assert.IsInstanceOfType(result, typeof(GetSubscriptionIdsResult));

        var subscriptions = (GetSubscriptionIdsResult)result;
        Assert.AreEqual(1, subscriptions.SubscriptionIDs.Count());
        Assert.AreEqual("SubscriptionTest", subscriptions.SubscriptionIDs.First());
    }

    [TestMethod]
    public void ItShouldReturnAnEmptyListWhenNoSubscriptionMatch()
    {
        var handler = new GetSubscriptionIdsQueryHandler(Context);
        var result = handler.Handle(new GetSubscriptionIdsQuery { QueryName = "UnknownQuery" }, default).Result;

        Assert.IsInstanceOfType(result, typeof(GetSubscriptionIdsResult));

        var subscriptions = (GetSubscriptionIdsResult)result;
        Assert.AreEqual(0, subscriptions.SubscriptionIDs.Count());
    }
}
