namespace FasTnT.Host.Endpoints.Interfaces;

public record ListCustomQueriesResult(IEnumerable<CustomQueryDefinitionResult> Queries) : IPaginableResult
{
    public int ElementsCount => Queries.Count();
}
