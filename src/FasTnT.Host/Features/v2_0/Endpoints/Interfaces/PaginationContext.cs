using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record PaginationContext(Pagination Pagination)
{
    public static ValueTask<PaginationContext> BindAsync(HttpContext context)
    {
        int.TryParse(context.Request.Query.FirstOrDefault(x => x.Key == "perPage").Value.FirstOrDefault("30"), out var perPage);
        int.TryParse(context.Request.Query.FirstOrDefault(x => x.Key == "nextPageToken").Value.FirstOrDefault("0"), out var startFrom);
        var pagination = new Pagination(perPage, startFrom);

        return ValueTask.FromResult(new PaginationContext(pagination));
    }
}

