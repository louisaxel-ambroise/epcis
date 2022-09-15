using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Store;
using FasTnT.Application.UseCases.Subscriptions;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingUnsubscribeCommand
{
    readonly static EpcisContext Context = Tests.Context.EpcisTestContext.GetContext(nameof(WhenHandlingUnsubscribeCommand));
    readonly static IEnumerable<IStandardQuery> Queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Subscriptions.Add(new StandardSubscription
        {
            Name = "TestSubscription",
            QueryName = Queries.First().Name
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAnUnubscribeResultAndSendANotification()
    {
        var subscription = "TestSubscription";
        var handler = new SubscriptionsUseCasesHandler(Context, null, new List<ISubscriptionListener>());
        handler.DeleteSubscriptionAsync(subscription, CancellationToken.None).Wait();
            
        Assert.AreEqual(0, Context.Subscriptions.Count());
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameDoesNotExist()
    {
        var subscription = "UnknownSubscription";
        var handler = new SubscriptionsUseCasesHandler(Context, null, new List<ISubscriptionListener>());

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.DeleteSubscriptionAsync(subscription, CancellationToken.None));
    }
}
