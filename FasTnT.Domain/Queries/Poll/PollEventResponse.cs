using FasTnT.Domain.Model;

namespace FasTnT.Domain.Queries.Poll
{
    public class PollEventResponse : PollResponse
    {
        public PollEventResponse(string queryName, List<Event> eventList)
        {
            QueryName = queryName;
            EventList = eventList;
        }
    }
}
