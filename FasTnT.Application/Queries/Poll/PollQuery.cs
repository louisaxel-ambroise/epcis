using System;
using System.Collections.Generic;

namespace FasTnT.Application.Queries.Poll
{
    public record PollQuery : IQuery<PollResponse>
    {
        public string QueryName { get; init; }
        public IEnumerable<QueryParameter> Parameters { get; init; }
    }

    public record QueryParameter
    {
        public string Name { get; init; }
        public string[] Values { get; init; } = Array.Empty<string>();
    }
}
