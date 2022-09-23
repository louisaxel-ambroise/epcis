using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Features.v2_0.Communication.Json.Formatters;

namespace FasTnT.Features.v2_0;

public static class ErrorHandlingFactory
{
    public static Delegate Create(Delegate handler)
    {
        return async (HttpContext context) =>
        {
            try
            {
                await RequestDelegateFactory.Create(handler, new() { ServiceProvider = context.RequestServices }).RequestDelegate.Invoke(context);
            }
            catch (Exception ex)
            {
                var error = ex is EpcisException epcisException ? epcisException : EpcisException.Default;

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = JsonResponseFormatter.GetHttpStatusCode(error);

                await context.Response.WriteAsync(JsonResponseFormatter.FormatError(error));
            }
        };
    }
}
