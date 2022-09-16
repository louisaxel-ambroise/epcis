using FasTnT.Domain.Model.Events;

namespace FasTnT.Domain.Model.Queries;

public class QueryResponse
{
    public string QueryName { get; init; }
    public string SubscriptionId { get; set; }
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }

    public QueryResponse(string queryName, List<Event> events, List<MasterData> vocabulary) : this(queryName, null, events, vocabulary)
    {
    }

    public QueryResponse(string queryName, string subscriptionId, List<Event> events, List<MasterData> vocabulary)
    {
        QueryName = queryName;
        SubscriptionId = subscriptionId;
        EventList = events;
        VocabularyList = vocabulary;
    }
}