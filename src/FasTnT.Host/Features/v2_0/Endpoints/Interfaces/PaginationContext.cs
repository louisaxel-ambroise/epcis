using FasTnT.Application.Domain.Model.Queries;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record PaginationContext(Pagination Pagination)
{
    public static ValueTask<PaginationContext> BindAsync(HttpContext context)
    {
        var perPage = int.Parse(context.Request.Query.FirstOrDefault(x => x.Key == "perPage").Value.FirstOrDefault("30"));
        var startFrom = int.Parse(context.Request.Query.FirstOrDefault(x => x.Key == "nextPageToken").Value.FirstOrDefault("0"));
        var pagination = new Pagination(perPage, startFrom);

        return ValueTask.FromResult(new PaginationContext(pagination));
    }
}

