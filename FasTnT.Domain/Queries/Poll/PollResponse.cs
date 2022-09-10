using FasTnT.Domain.Model;

namespace FasTnT.Domain.Queries.Poll;

public interface IEpcisResponse { }

public abstract class PollResponse : IEpcisResponse
{
    public string QueryName { get; init; }
    public string SubscriptionId { get; set; }
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }
}

public class PollEventResponse : PollResponse
{
    public PollEventResponse(string queryName, List<Event> eventList)
    {
        QueryName = queryName;
        EventList = eventList;
    }
}

public class PollMasterdataResponse : PollResponse
{
    public PollMasterdataResponse(string queryName, List<MasterData> vocabularyList)
    {
        QueryName = queryName;
        VocabularyList = vocabularyList;
    }
}