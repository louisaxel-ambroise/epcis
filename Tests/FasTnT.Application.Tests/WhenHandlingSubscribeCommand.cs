using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Application.UseCases.StoreStandardQuerySubscription;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingSubscribeCommand
{
    readonly static EpcisContext Context = Tests.Context.EpcisTestContext.GetContext(nameof(Tests.WhenHandlingSubscribeCommand));
    readonly static IEnumerable<IStandardQuery> Queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Subscriptions.Add(new Subscription
        {
            Name = "TestSubscription",
            QueryName = Queries.First().Name
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameAlreadyExist()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            QueryName = "SimpleEventQuery"
        };
        var handler = new StoreStandardQuerySubscriptionHandler(Context, Queries);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StoreSubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryNameDoesNotExist()
    {
        var subscription = new Subscription
        {
            Name = "InvalidSubscription",
            QueryName = "UnknownQuery"
        };
        var handler = new StoreStandardQuerySubscriptionHandler(Context, Queries);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StoreSubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryDoesNotAllowSubscription()
    {
        var subscription = new Subscription
        {
            Name = "MasterdataTestSubscription",
            QueryName = "SimpleMasterdataQuery"
        };
        var handler = new StoreStandardQuerySubscriptionHandler(Context, Queries);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StoreSubscriptionAsync(subscription, CancellationToken.None));
    }
}
