using FasTnT.Domain.Queries;

namespace FasTnT.Host.Features.v2_0;

public record QueryParameters(IEnumerable<QueryParameter> Parameters)
{
    public static ValueTask<QueryParameters> BindAsync(HttpContext context)
    {
        var parameters = context.Request.Query.Select(x => new QueryParameter(x.Key, x.Value));

        return ValueTask.FromResult(new QueryParameters(parameters));
    }
}

