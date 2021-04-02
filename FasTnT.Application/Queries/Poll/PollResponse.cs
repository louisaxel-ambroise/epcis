using FasTnT.Domain.Model;
using System.Collections.Generic;

namespace FasTnT.Application.Queries.Poll
{
    public record PollResponse
    {
        public string QueryName { get; init; }
        public string SubscriptionId { get; init; }
        public IList<Event> EventList { get; init; } = new List<Event>();
        public IList<MasterData> MasterdataList { get; init; } = new List<MasterData>();
    }
}
