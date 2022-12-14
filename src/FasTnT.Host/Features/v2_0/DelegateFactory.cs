using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Host.Features.v2_0.Communication.Json.Formatters;

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

                var handler = handlerProvider(context);

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

    private static IDictionary<string, string> ParseEpcisHeaders(HttpRequest request)
    {
        return request.Headers.Where(x => x.Key.StartsWith("GS1-") && x.Key != "GS1-EPCIS-Extensions").ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
    }

    private static void SetResponseHeaders(HttpResponse response)
    {
        response.Headers.Add("GS1-EPCIS-Version", "2.0");
        response.Headers.Add("GS1-EPCIS-Min", "2.0");
        response.Headers.Add("GS1-EPCIS-Max", "2.0");
        response.Headers.Add("GS1-EPC-Format", "Never_Translates");
        response.Headers.Add("GS1-EPCIS-Capture-Limit", Constants.Instance.MaxEventsCapturePerCall.ToString());
        response.Headers.Add("GS1-Vendor-Version", Constants.Instance.VendorVersion);
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
