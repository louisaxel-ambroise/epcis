using FasTnT.Application.Services;
using FasTnT.Domain.Model;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions
{
    public class SubscriptionRunner
    {
        private readonly IEnumerable<IEpcisQuery> _epcisQueries;
        private readonly EpcisContext _context;
        private readonly ISubscriptionResultSender _resultSender;

        public SubscriptionRunner(IEnumerable<IEpcisQuery> epcisQueries, EpcisContext context, ISubscriptionResultSender resultSender)
        {
            _epcisQueries = epcisQueries;
            _context = context;
            _resultSender = resultSender;
        }

        public async Task Run(Subscription subscription, CancellationToken cancellationToken)
        {
            _context.Attach(subscription);

            var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = System.DateTime.UtcNow, ResultsSent = true, Successful = true };
            var query = _epcisQueries.Single(x => x.Name == subscription.QueryName);
            var pendingRequests = await _context.PendingRequests.Where(x => x.SubscriptionId == subscription.Id).ToListAsync(cancellationToken);
            PollResponse response = new PollEventResponse(query.Name, new());

            if (pendingRequests.Any())
            {
                var parameters = subscription.Parameters
                    .Select(s => new QueryParameter(s.Name, s.Value))
                    .Append(new ("EQ_requestId", pendingRequests.Select(x => x.RequestId.ToString()).ToArray()))
                    .ToArray();

                response = await query.HandleAsync(parameters, cancellationToken);
            }

            response.SubscriptionId = subscription.Name;

            var resultsSent = await SendSubscriptionResults(subscription, response, cancellationToken).ConfigureAwait(false);

            if (resultsSent)
            {
                _context.PendingRequests.RemoveRange(pendingRequests);
            }
            else
            {
                executionRecord.Successful = false;
                executionRecord.Reason = "Failed to send subscription result";
            }

            subscription.ExecutionRecords.Add(executionRecord);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> SendSubscriptionResults(Subscription subscription, PollResponse response, CancellationToken cancellationToken)
        {
            if (response.EventList.Count > 0 || subscription.RecordIfEmpty)
            {
                return await _resultSender.Send(subscription.Destination, response, cancellationToken);
            }

            return true;
        }
    }
}
