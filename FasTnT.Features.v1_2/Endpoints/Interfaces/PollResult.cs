using FasTnT.Domain.Model;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces.Queries;

public record PollResult
{
    public PollResult(QueryResponse response)
    {
        QueryName = response.QueryName;
        SubscriptionId = response.SubscriptionId;
        EventList = response.EventList;
        VocabularyList = response.VocabularyList;
    }

    public string QueryName { get; init; }
    public string SubscriptionId { get; set; }
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }
}
