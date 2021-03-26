using FasTnT.Application.Queries.Poll;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Services
{
    public interface IEpcisQuery
    {
        public string Name { get; }
        public bool AllowSubscription { get; }

        Task<PollResponse> HandleAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
    }
}
