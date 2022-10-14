using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;

static class Options
{
    public static readonly Action<AuthorizationOptions> AuthorizationPolicies = (options) =>
    { 
        options.AddPolicy("query", policy => policy.RequireClaim("fastnt.query"));
        options.AddPolicy("capture", policy => policy.RequireClaim("fastnt.capture"));
    };

    public static readonly Action<HttpLoggingOptions> LoggingPolicy = (options) =>
    {
        options.LoggingFields = HttpLoggingFields.All;
        options.MediaTypeOptions.AddText("application/xml");
        options.MediaTypeOptions.AddText("application/json");
        options.MediaTypeOptions.AddText("application/text+xml");
        options.RequestBodyLogLimit = 4096;
        options.ResponseBodyLogLimit = 4096;
    };
}
