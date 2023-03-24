using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Services.Subscriptions;
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