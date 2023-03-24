using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record CreateCustomQueryRequest(StoredQuery Query)
{
    public static async ValueTask<CreateCustomQueryRequest> BindAsync(HttpContext context)
    {
        var request = await JsonRequestParser.ParseCustomQueryRequestAsync(context.Request.Body, context.RequestAborted);

        return request;
    }

    public static implicit operator CreateCustomQueryRequest(StoredQuery query) => new(query);
}