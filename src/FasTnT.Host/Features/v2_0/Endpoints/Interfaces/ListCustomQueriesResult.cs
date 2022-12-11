namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record ListCustomQueriesResult(IEnumerable<CustomQueryDefinitionResult> Queries) : IPaginableResult
{
    public int ElementsCount => Queries.Count();
}
