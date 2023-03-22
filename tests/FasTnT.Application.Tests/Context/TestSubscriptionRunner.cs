using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Context;

public class TestSubscriptionRunner : ISubscriptionRunner
{
    public Task RunAsync(SubscriptionContext executionContext, DateTime executionTime, CancellationToken cancellationToken)
    {
        executionContext.SendQueryResults(new QueryResponse("testQuery", new List<Event>()), cancellationToken);

        return Task.CompletedTask;
    }
}