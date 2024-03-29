using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Endpoints.Interfaces;

public record QueryResult(QueryResponse Response) : IPaginableResult
{
    public int ElementsCount => Response.EventList.Count;
}