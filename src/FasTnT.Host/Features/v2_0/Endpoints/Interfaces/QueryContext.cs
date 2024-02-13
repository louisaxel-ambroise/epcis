using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record QueryContext(IEnumerable<QueryParameter> Parameters)
{
    static readonly List<QueryParameter> Default = [QueryParameter.Create("perPage", "30")];
    static readonly string[] ReservedKeywords = ["auth", "stream"];

    public static ValueTask<QueryContext> BindAsync(HttpContext context)
    {
        var queryParameters = context.Request.Query.Where(x => !ReservedKeywords.Contains(x.Key));
        var parameters = Default.Union(queryParameters.Select(x => QueryParameter.Create(x.Key, x.Value.ToArray())));

        return ValueTask.FromResult(new QueryContext(parameters));
    }
}

