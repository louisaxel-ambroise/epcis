using FasTnT.Application.Queries.GetQueryNames;
using FasTnT.Application.Queries.Poll;
using FasTnT.Application.Services;
using FasTnT.Application.Subscriptions;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Commands.Subscribe;
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
    public class WhenHandlingSubscribeCommand
    {
        readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(Tests.WhenHandlingSubscribeCommand));
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
        public void ItShouldReturnASubscribeResultAndSendANotification()
        {
            var subscription = new SubscribeCommand
            {
                SubscriptionId = "NewSubscription",
                QueryName = "SimpleEventQuery"
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
}
