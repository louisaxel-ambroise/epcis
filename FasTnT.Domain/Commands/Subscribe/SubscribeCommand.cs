using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Domain.Commands.Subscribe;

public class SubscribeCommand : IRequest<IEpcisResponse>
{
    public string SubscriptionId { get; init; }
    public string QueryName { get; init; }
    public string Trigger { get; init; }
    public string Destination { get; init; }
    public bool ReportIfEmpty { get; init; }
    public DateTime? InitialRecordTime { get; init; }
    public QuerySchedule Schedule { get; init; }
    public List<QueryParameter> Parameters { get; init; } = new();
}
