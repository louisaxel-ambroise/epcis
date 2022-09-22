using FasTnT.Application.Services.Users;
using FasTnT.Domain.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;

static class Options
{
    public static readonly ExceptionHandlerOptions ExceptionHandler = new()
    {
        ExceptionHandler = (HttpContext ctx) => Task.Run(() =>
        {
            var exceptionHandler = ctx.Features.Get<IExceptionHandlerPathFeature>();

            ctx.Response.StatusCode = exceptionHandler?.Error switch
            {
                EpcisException _ => 400,
                FormatException _ => 415,
                _ => 500
            };
        })
    };

    public static readonly Action<AuthorizationOptions> AuthorizationPolicies = (options) =>
    {
        options.AddPolicy(nameof(ICurrentUser.CanQuery), policy => policy.RequireClaim(nameof(ICurrentUser.CanQuery), bool.TrueString));
        options.AddPolicy(nameof(ICurrentUser.CanCapture), policy => policy.RequireClaim(nameof(ICurrentUser.CanCapture), bool.TrueString));
    };

    public static readonly Action<HttpLoggingOptions> LoggingPolicy = (options) =>
    {
        options.LoggingFields = HttpLoggingFields.All;
        options.MediaTypeOptions.AddText("application/xml");
        options.MediaTypeOptions.AddText("application/text+xml");
        options.RequestBodyLogLimit = 4096;
        options.ResponseBodyLogLimit = 4096;
    };
}
