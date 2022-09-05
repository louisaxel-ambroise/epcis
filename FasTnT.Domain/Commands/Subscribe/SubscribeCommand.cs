using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Domain.Commands.Subscribe;

public class SubscribeCommand : IRequest<IEpcisResponse>
{
    public string SubscriptionId { get; init; }
    public string QueryName { get; init; }
    public string Trigger { get; init; }
    public string Destination { get; set; }
    public bool ReportIfEmpty { get; set; }
    public DateTime? InitialRecordTime { get; set; }
    public QuerySchedule Schedule { get; init; } = new();
    public List<QueryParameter> Parameters { get; init; } = new();
}
