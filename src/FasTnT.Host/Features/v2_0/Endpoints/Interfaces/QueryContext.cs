using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record QueryContext(IEnumerable<QueryParameter> Parameters)
{
    static readonly List<QueryParameter> Default = new() { QueryParameter.Create("perPage", "30") };

    public static ValueTask<QueryContext> BindAsync(HttpContext context)
    {
        var parameters = Default.Union(context.Request.Query.Select(x => QueryParameter.Create(x.Key, x.Value.ToArray())));

        return ValueTask.FromResult(new QueryContext(parameters));
    }
}

