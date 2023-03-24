using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Masterdata;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

public record PollResult
{
    public PollResult(string queryName, List<Event> eventList)
    {
        QueryName = queryName;
        EventList = eventList;
    }

    public PollResult(string queryName, List<MasterData> vocabularyList)
    {
        QueryName = queryName;
        VocabularyList = vocabularyList;
    }

    public PollResult(string queryName, string subscriptionId, List<Event> eventList) : this(queryName, eventList)
    {
        SubscriptionId = subscriptionId;
    }

    public string QueryName { get; init; }
    public string SubscriptionId { get; set; }
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }
}
