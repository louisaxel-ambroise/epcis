using FasTnT.Domain.Model.Queries;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record QueryParameters(IEnumerable<QueryParameter> Parameters)
{
    public static ValueTask<QueryParameters> BindAsync(HttpContext context)
    {
        var parameters = context.Request.Query.Select(x => QueryParameter.Create(x.Key, x.Value.ToArray()));

        return ValueTask.FromResult(new QueryParameters(parameters));
    }
}

