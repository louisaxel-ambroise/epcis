using FasTnT.Domain.Model.Queries;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record QueryContext(IEnumerable<QueryParameter> Parameters)
{
    public static ValueTask<QueryContext> BindAsync(HttpContext context)
    {
        var parameters = context.Request.Query.Select(x => QueryParameter.Create(x.Key, x.Value.ToArray()));

        if(!parameters.Any(x => x.Name == "perPage"))
        {
            parameters = parameters.Append(new QueryParameter { Name = "perPage", Values = new[] { "30" } });
        }

        return ValueTask.FromResult(new QueryContext(parameters));
    }
}

