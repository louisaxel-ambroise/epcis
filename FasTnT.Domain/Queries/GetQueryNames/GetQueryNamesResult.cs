namespace FasTnT.Domain.Queries;

public record GetQueryNamesResult(IEnumerable<string> QueryNames) : IEpcisResponse;
