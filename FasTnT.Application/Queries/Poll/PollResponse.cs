using FasTnT.Domain.Model;
using System.Collections.Generic;

namespace FasTnT.Application.Queries.Poll
{
    public record PollResponse
    {
        public string QueryName { get; init; }
        public string SubscriptionId { get; init; }
        public List<Event> EventList { get; init; } = new();
        public List<MasterData> MasterdataList { get; init; } = new();
    }
}
