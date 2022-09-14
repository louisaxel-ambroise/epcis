using FasTnT.Domain.Model.Events;

namespace FasTnT.Domain.Model.Queries;

public class QueryResponse
{
    public string QueryName { get; init; }
    public string SubscriptionId { get; set; }
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }

    public static QueryResponse EventsResponse(string name, IEnumerable<Event> events)
    {
        return new QueryResponse
        {
            QueryName = name,
            EventList = events.ToList()
        };
    }

    public static QueryResponse MasterdataResponse(string name, IEnumerable<MasterData> masterdata)
    {
        return new QueryResponse
        {
            QueryName = name,
            VocabularyList = masterdata.ToList()
        };
    }

    public static QueryResponse Empty(string name) => EventsResponse(name, Array.Empty<Event>());
}