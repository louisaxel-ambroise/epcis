using FasTnT.Application.Services;
using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Application.Subscriptions;
using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Notifications;
using MediatR;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingSubscribeCommand
{
    readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(Tests.WhenHandlingSubscribeCommand));
    readonly static IEnumerable<IEpcisQuery> Queries = new IEpcisQuery[] { new SimpleEventQuery(Context), new SimpleMasterDataQuery(Context) };
    readonly static Mock<IMediator> Mediator = new (MockBehavior.Loose);

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
    public void ItShouldReturnASubscribeResultAndSendANotification()
    {
        var subscription = new SubscribeCommand
        {
            SubscriptionId = "NewSubscription",
            QueryName = "SimpleEventQuery",
            Trigger = "testTrigger"
        };
        var handler = new SubscribeCommandHandler(Context, Queries, Mediator.Object);
        var result = handler.Handle(subscription, CancellationToken.None).Result;
            
        Assert.IsNotNull(result);
        Mediator.Verify(x => x.Publish(It.Is<SubscriptionCreatedNotification>(x => x.SubscriptionId == Context.Subscriptions.Single(x => x.Name == "NewSubscription").Id), It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameAlreadyExist()
    {
        var subscription = new SubscribeCommand
        {
            SubscriptionId = "TestSubscription",
            QueryName = "SimpleEventQuery"
        };
        var handler = new SubscribeCommandHandler(Context, Queries, Mediator.Object);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.Handle(subscription, CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryNameDoesNotExist()
    {
        var subscription = new SubscribeCommand
        {
            SubscriptionId = "InvalidSubscription",
            QueryName = "UnknownQuery"
        };
        var handler = new SubscribeCommandHandler(Context, Queries, Mediator.Object);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.Handle(subscription, CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryDoesNotAllowSubscription()
    {
        var subscription = new SubscribeCommand
        {
            SubscriptionId = "MasterdataTestSubscription",
            QueryName = "SimpleMasterdataQuery"
        };
        var handler = new SubscribeCommandHandler(Context, Queries, Mediator.Object);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.Handle(subscription, CancellationToken.None));
    }
}
