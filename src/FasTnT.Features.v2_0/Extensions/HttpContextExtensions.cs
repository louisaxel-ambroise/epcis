using System.Collections.Specialized;

namespace FasTnT.Features.v2_0.Extensions;

public static class HttpContextExtensions
{
    public const string PerPage = "perPage";

    public static bool IsPaginated(this HttpContext context)
    {
        return context.Request.Query.Any(x => x.Key.Equals(PerPage));
    }

    public static int GetPerPageValue(this HttpContext context)
    {
        return int.Parse(context.Request.Query.SingleOrDefault(x => x.Key.Equals(PerPage)).Value.SingleOrDefault("30"));
    }

    public static string BuildNextLink(this HttpContext context, NameValueCollection queryString)
    {
        return context.Request.Scheme + "://" + context.Request.Host + context.Request.Path + "?" + queryString;
    }
}
