using FasTnT.Domain.Model;
using System.Collections.Generic;

namespace FasTnT.Domain.Queries.Poll
{
    public class PollResponse 
    {
        public string QueryName { get; init; }
        public string SubscriptionId { get; set; }
        public List<Event> EventList { get; init; }
        public List<MasterData> VocabularyList { get; init; }

        public PollResponse(string queryName) => QueryName = queryName;
    }
}
