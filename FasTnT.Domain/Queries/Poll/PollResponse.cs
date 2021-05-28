using FasTnT.Domain.Model;
using System.Collections.Generic;

namespace FasTnT.Application.Queries.Poll
{
    public record PollResponse(string QueryName, string SubscriptionId, List<Event> EventList, List<MasterData> MasterdataList);
}
