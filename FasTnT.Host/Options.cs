using FasTnT.Application.Services.Users;
using FasTnT.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;

static class Options
{
    public static ExceptionHandlerOptions ExceptionHandler = new()
    {
        ExceptionHandler = (ctx) =>
        {
            var exceptionHandler = ctx.Features.Get<IExceptionHandlerPathFeature>();

            ctx.Response.StatusCode = exceptionHandler?.Error is EpcisException
                ? 400
                : 500;

            return Task.CompletedTask;
        }
    };

    public static Action<AuthorizationOptions> AuthorizationPolicies = (options) =>
    {
        options.AddPolicy(nameof(ICurrentUser.CanQuery), policy => policy.RequireClaim(nameof(ICurrentUser.CanQuery), bool.TrueString));
        options.AddPolicy(nameof(ICurrentUser.CanCapture), policy => policy.RequireClaim(nameof(ICurrentUser.CanCapture), bool.TrueString));
    };

    public static Action<HttpLoggingOptions> LoggingPolicy = (options) =>
    {
        options.LoggingFields = HttpLoggingFields.All;
        options.MediaTypeOptions.AddText("application/xml");
        options.MediaTypeOptions.AddText("application/text+xml");
        options.RequestBodyLogLimit = 4096;
        options.ResponseBodyLogLimit = 4096;
    };
}