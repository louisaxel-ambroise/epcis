using FasTnT.Domain.Model.Queries;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record QueryResult(QueryResponse Response) : IPaginableResult
{
    public int ElementsCount => Response.EventList.Count;
}