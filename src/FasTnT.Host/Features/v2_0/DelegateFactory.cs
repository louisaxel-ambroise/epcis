using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Host.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;
using Microsoft.Extensions.Options;

namespace FasTnT.Host.Features.v2_0;

public static class DelegateFactory
{
    public static Delegate Create(Func<HttpContext, Delegate> handlerProvider)
    {
        return async (HttpContext context) =>
        {
            var epcisHeaders = ParseEpcisHeaders(context.Request);

            try
            {
                ValidateHeaders(epcisHeaders);
                SetResponseHeaders(context.Response);

                var options = new RequestDelegateFactoryOptions { ServiceProvider = context.RequestServices };
                var factory = RequestDelegateFactory.Create(handlerProvider(context), options);

                await factory.RequestDelegate.Invoke(context);
            }
            catch (Exception ex)
            {
                var error = ex is EpcisException epcisException ? epcisException : EpcisException.Default;

                if (context.Request.Headers.Accept.Any(x => x.Contains("+xml") || x.Contains("/xml")))
                {
                    context.Response.ContentType = "application/problem+xml";
                    context.Response.StatusCode = XmlResponseFormatter.GetHttpStatusCode(error);

                    var formatted = XmlResponseFormatter.FormatError(error);

                    await context.Response.WriteAsync(formatted.ToString(), context.RequestAborted);
                }
                else
                {
                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = JsonResponseFormatter.GetHttpStatusCode(error);

                    await context.Response.WriteAsync(JsonResponseFormatter.FormatError(error), context.RequestAborted);
                }
            }
        };
    }

    private static Dictionary<string, string> ParseEpcisHeaders(HttpRequest request)
    {
        return request.Headers.Where(x => x.Key.StartsWith("GS1-") && x.Key != "GS1-EPCIS-Extensions").ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
    }

    private static void SetResponseHeaders(HttpResponse response)
    {
        var constants = response.HttpContext.RequestServices.GetService<IOptions<Constants>>().Value;

        response.Headers.Append("GS1-EPCIS-Version", "2.0");
        response.Headers.Append("GS1-EPCIS-Min", "2.0");
        response.Headers.Append("GS1-EPCIS-Max", "2.0");
        response.Headers.Append("GS1-EPC-Format", "Never_Translates");
        response.Headers.Append("GS1-EPCIS-Capture-Limit", constants.MaxEventsCapturePerCall.ToString());
        response.Headers.Append("GS1-Vendor-Version", constants.VendorVersion.ToString());

        if (constants.CaptureSizeLimit > 0)
        {
            response.Headers.Append("GS1-EPCIS-Capture-File-SizeLimit", constants.CaptureSizeLimit.ToString());
        }
    }

    private static void ValidateHeaders(IDictionary<string, string> epcisHeaders)
    {
        if (epcisHeaders.TryGetValue("GS1-EPC-Format", out var epcFormat) && epcFormat != "Never_Translates")
        {
            throw new EpcisException(ExceptionType.ImplementationException, "Only 'Never_Translates' is supported for GS1-EPC-Format");
        }
        if (epcisHeaders.TryGetValue("GS1-Capture-Error-Behaviour", out var captureErrorBehaviour) && captureErrorBehaviour != "rollback")
        {
            throw new EpcisException(ExceptionType.ImplementationException, "Only 'rollback' is supported for GS1-Capture-Error-Behaviour");
        }
    }
}
