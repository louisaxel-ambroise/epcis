using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Application.Services
{
    public interface IEpcisQuery
    {
        public string Name { get; }
        public bool AllowSubscription { get; }

        Task<PollResponse> HandleAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
    }
}
