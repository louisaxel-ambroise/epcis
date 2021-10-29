using FasTnT.Domain.Model;

namespace FasTnT.Domain.Queries.Poll
{
    public abstract class PollResponse 
    {
        public string QueryName { get; init; }
        public string SubscriptionId { get; set; }
        public List<Event> EventList { get; init; }
        public List<MasterData> VocabularyList { get; init; }
    }
}
