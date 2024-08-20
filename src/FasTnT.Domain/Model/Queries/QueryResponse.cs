using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Domain.Model.Queries;

public class QueryResponse
{
    public string QueryName { get; set; }
    public string SubscriptionName { get; set; }
    public List<Event> EventList { get; set; }
    public List<MasterData> VocabularyList { get; set; }

    public QueryResponse(string queryName, QueryData queryData, string subscriptionName = null)
    {
        QueryName = queryName;
        SubscriptionName = subscriptionName;
        EventList = queryData.EventList;
        VocabularyList = queryData.VocabularyList;
    }
}
