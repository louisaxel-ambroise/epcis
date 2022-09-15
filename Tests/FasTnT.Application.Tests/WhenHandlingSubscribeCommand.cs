using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Application.UseCases.Subscriptions;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingSubscribeCommand
{
    readonly static EpcisContext Context = Tests.Context.EpcisTestContext.GetContext(nameof(WhenHandlingSubscribeCommand));
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
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameAlreadyExist()
    {
        var subscription = new StandardSubscription
        {
            Name = "TestSubscription",
            QueryName = "SimpleEventQuery"
        };
        var handler = new SubscriptionsUseCasesHandler(Context, Queries, null);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StandardQuerySubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryNameDoesNotExist()
    {
        var subscription = new StandardSubscription
        {
            Name = "InvalidSubscription",
            QueryName = "UnknownQuery"
        };
        var handler = new SubscriptionsUseCasesHandler(Context, Queries, null);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StandardQuerySubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryDoesNotAllowSubscription()
    {
        var subscription = new StandardSubscription
        {
            Name = "MasterdataTestSubscription",
            QueryName = "SimpleMasterdataQuery"
        };
        var handler = new SubscriptionsUseCasesHandler(Context, Queries, null);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StandardQuerySubscriptionAsync(subscription, CancellationToken.None));
    }
}
