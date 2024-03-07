using FasTnT.Domain.Exceptions;
using FasTnT.Host.Communication.Xml.Formatters;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces.Utils;

namespace FasTnT.Host.Features.v1_2;

public static class ErrorHandlingFactory
{
    public static Delegate HandleSoap(Delegate handler)
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

                context.Response.StatusCode = 400;
                await context.Response.FormatSoapFault(error, context.RequestAborted);
            }
        };
    }

    public static Delegate HandleError(Delegate handler)
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
                context.Response.StatusCode = SoapResponseFormatter.GetHttpStatusCode(error);

                await context.Response.WriteAsync(SoapResponseFormatter.FormatError(error).ToString(SaveOptions.None));
            }
        };
    }
}