using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Domain.Model.Queries;

public class QueryResponse
{
    public string QueryName { get; set; }
    public string SubscriptionId { get; set; }
    public List<Event> EventList { get; set; }
    public List<MasterData> VocabularyList { get; set; }

    public QueryResponse(string queryName, QueryData queryData) : this(queryName, null, queryData)
    {
    }

    public QueryResponse(string queryName, string subscriptionId, QueryData queryData)
    {
        QueryName = queryName;
        SubscriptionId = subscriptionId;
        EventList = queryData.EventList;
        VocabularyList = queryData.VocabularyList;
    }
}
