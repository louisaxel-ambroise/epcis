namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record CollectionResult(IEnumerable<string> Values) : IPaginableResult
{
    public int ElementsCount => Values.Count();
}
