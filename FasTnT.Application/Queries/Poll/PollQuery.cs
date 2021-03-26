using FasTnT.Domain.Model;
using System;
using System.Collections.Generic;

namespace FasTnT.Application.Queries.Poll
{
    public class PollQuery : IQuery<PollResponse>
    {
        public string QueryName { get; init; }
        public IEnumerable<QueryParameter> Parameters { get; init; }
    }

    public class PollResponse
    {
        public string QueryName { get; init; }
        public string SubscriptionId { get; init; }
        public IList<Event> EventList { get; init; } = new List<Event>();
        public IList<MasterData> MasterdataList { get; init; } = new List<MasterData>();
    }

    public class QueryParameter
    {
        public string Name { get; init; }
        public string[] Values { get; init; } = Array.Empty<string>();
    }
}
