using FasTnT.Application.Queries.Poll;
using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Infrastructure.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FasTnT.Application.Tests
{
    [TestClass]
    public class WhenHandlingPollQuery
    {
        readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(Tests.WhenHandlingPollQuery));
        readonly static ICurrentUser UserContext = new TestCurrentUser();

        [TestMethod]
        public void ItShouldReturnAllTheQueryNames()
        {
            var queries = new IEpcisQuery[] { new SimpleEventQuery(Context, UserContext), new SimpleMasterDataQuery(Context) };
            var handler = new PollQueryHandler(queries);
            var request = new PollQuery("SimpleEventQuery", new List<QueryParameter>());
            var result = handler.Handle(request, default).Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ItShouldThrowAnExceptionIfTheQueryDoesNotExist()
        {
            var queries = new IEpcisQuery[] { new SimpleEventQuery(Context, UserContext), new SimpleMasterDataQuery(Context) };
            var handler = new PollQueryHandler(queries);
            var request = new PollQuery("UnknownQuery", new List<QueryParameter>());

            Assert.ThrowsExceptionAsync<EpcisException>(() => handler.Handle(request, default));
        }
    }
}
