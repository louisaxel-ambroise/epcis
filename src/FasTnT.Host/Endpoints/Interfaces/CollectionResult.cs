namespace FasTnT.Host.Endpoints.Interfaces;

public record CollectionResult(IEnumerable<string> Values) : IPaginableResult
{
    public int ElementsCount => Values.Count();
}
