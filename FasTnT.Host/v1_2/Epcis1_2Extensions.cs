using FasTnT.Formatter.Xml;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FasTnT.Formatter.Xml.Parsers;

namespace FasTnT.Host.v1_2
{
    public static class Epcis1_2Extensions
    {
        public static IApplicationBuilder UseQueryEpcis1_2(this IApplicationBuilder app, string path)
        {
            return app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet(path, GenerateWsdl);
                endpoints.MapPost(path, ProcessQuery);
            });
        }

        public static IApplicationBuilder UseCaptureEpcis1_2(this IApplicationBuilder app, string path)
        {
            return app.UseEndpoints(endpoints => endpoints.MapPost(path, ProcessCapture));
        }

        private static Task GenerateWsdl(HttpContext context)
        {
            return context.Response.WriteAsync("WSDL");
        }

        private static async Task ProcessCapture(HttpContext context)
        {
            var request = await CaptureRequestParser.ParseEpcisDocumentAsync(context.Request.Body, context.RequestAborted);
            await HandleRequest(context, request);
        }

        private static async Task ProcessQuery(HttpContext context)
        {
            await HandleRequest(context, QueryRequestParser.ParseQuery(context.Request.Body));

        }

        private static async Task HandleRequest(HttpContext context, IBaseRequest request)
        {
            var mediator = context.RequestServices.GetService<IMediator>();
            var response = await mediator.Send(request);
         
            await context.Response.WriteAsync(XmlResponseFormatter.Format(response));
        }
    }
}
