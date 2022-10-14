using FasTnT.Domain.Model.Queries;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record QueryContext(IEnumerable<QueryParameter> Parameters)
{
    public static ValueTask<QueryContext> BindAsync(HttpContext context)
    {
        var parameters = context.Request.Query.Select(x => QueryParameter.Create(x.Key, x.Value.ToArray()));

        return ValueTask.FromResult(new QueryContext(parameters));
    }
}

