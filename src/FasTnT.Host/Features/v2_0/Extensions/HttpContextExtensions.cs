namespace FasTnT.Host.Features.v2_0.Extensions;

public static class HttpContextExtensions
{
    public static string PerPage => "perPage";
    public static string DefaultPageSize => "30";

    public static bool IsPaginated(this HttpContext context)
    {
        return context.Request.Query.Any(x => x.Key.Equals(PerPage));
    }

    public static int GetPerPageValue(this HttpContext context)
    {
        return int.Parse(context.Request.Query.TryGetValue(PerPage, out var value) ? value : DefaultPageSize);
    }

    public static string BuildNextLink(this HttpContext context, ICollection queryString)
    {
        return context.Request.Scheme + "://" + context.Request.Host + context.Request.Path + "?" + queryString;
    }
}
