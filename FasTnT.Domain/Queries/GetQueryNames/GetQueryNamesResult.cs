using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Queries.GetQueryNames;

public record GetQueryNamesResult(IEnumerable<string> QueryNames) : IEpcisResponse;
