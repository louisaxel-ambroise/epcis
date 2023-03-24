using FasTnT.Application.Domain.Model.Queries;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record QueryResult(QueryResponse Response) : IPaginableResult
{
    public int ElementsCount => Response.EventList.Count;
}