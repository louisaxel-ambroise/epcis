using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Host.Features.v1_2.Communication.Formatters;

namespace FasTnT.Host.Features.v1_2;

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

                context.Response.ContentType = "application/xml";
                context.Response.StatusCode = XmlResponseFormatter.GetHttpStatusCode(error);

                await context.Response.WriteAsync(XmlResponseFormatter.FormatError(error).ToString(SaveOptions.None));
            }
        };
    }
}