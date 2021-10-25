using FasTnT.Application.Queries.GetQueryNames;
using FasTnT.Application.Queries.Poll;
using FasTnT.Application.Services;
using FasTnT.Application.Subscriptions;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Notifications;
using FasTnT.Infrastructure.Database;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FasTnT.Application.Tests
{
    [TestClass]
    public class WhenHandlingUnsubscribeCommand
    {
        readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(Tests.WhenHandlingUnsubscribeCommand));
        readonly static IEnumerable<IEpcisQuery> Queries = new IEpcisQuery[] { new SimpleEventQuery(Context), new SimpleMasterDataQuery(Context) };
        readonly static Mock<IMediator> Mediator = new (MockBehavior.Loose);

        [ClassInitialize]
        public static void Initialize(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _)
        {
            Context.Subscriptions.Add(new Domain.Model.Subscription
            {
                Name = "TestSubscription",
                QueryName = Queries.First().Name
            });

            Context.SaveChanges();
        }

        [TestMethod]
        public void ItShouldReturnAnUnubscribeResultAndSendANotification()
        {
            var subscriptionId = Context.Subscriptions.First().Id;
            var subscription = new UnsubscribeCommand
            {
                SubscriptionId = "TestSubscription"
            };
            var handler = new UnsubscribeCommandHandler(Context, Mediator.Object);
            var result = handler.Handle(subscription, CancellationToken.None).Result;
            
            Assert.IsNotNull(result);
            Mediator.Verify(x => x.Publish(It.Is<SubscriptionRemovedNotification>(x => x.SubscriptionId == subscriptionId), It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameDoesNotExist()
        {
            var subscription = new UnsubscribeCommand
            {
                SubscriptionId = "UnknownSubscription"
            };
            var handler = new UnsubscribeCommandHandler(Context, Mediator.Object);

            Assert.ThrowsExceptionAsync<EpcisException>(() => handler.Handle(subscription, CancellationToken.None));
        }
    }
}
