using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Features.v2_0.Communication.Json.Parsers;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record CreateCustomQueryRequest(CustomQuery Query)
{
    public static async ValueTask<CreateCustomQueryRequest> BindAsync(HttpContext context)
    {
        var request = await JsonRequestParser.ParseCustomQueryRequestAsync(context.Request.Body, context.RequestAborted);

        return request;
    }

    public static implicit operator CreateCustomQueryRequest(CustomQuery query) => new(query);
}