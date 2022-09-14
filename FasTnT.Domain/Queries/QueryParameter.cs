using FasTnT.Domain.Model;

namespace FasTnT.Domain.Queries.Poll;

public class QueryParameter 
{
    public string Name { get; set; }
    public string[] Values { get; set; }

    public static QueryParameter Create(string name, IEnumerable<string> values) => new() { Name = name, Values = values.ToArray() };
}

public class QueryResponse
{
    public string QueryName { get; init; }
    public string SubscriptionId { get; set; }
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }

    public static QueryResponse Events(string name, IEnumerable<Event> events)
    {
        return new QueryResponse
        {
            QueryName = name,
            EventList = events.ToList()
        };
    }

    public static QueryResponse Masterdata(string name, IEnumerable<MasterData> masterdata)
    {
        return new QueryResponse
        {
            QueryName = name,
            VocabularyList = masterdata.ToList()
        };
    }

    public static QueryResponse Empty(string name)
    {
        return new QueryResponse
        {
            QueryName = name,
            EventList = new()
        };
    }
}