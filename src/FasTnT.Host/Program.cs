using FasTnT.Application;
using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Host.Endpoints;
using FasTnT.Host.Services.Database;
using FasTnT.Host.Services.User;
using FasTnT.Host.Subscriptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(BasicAuthentication.SchemeName).AddScheme<AuthenticationSchemeOptions, BasicAuthentication>(BasicAuthentication.SchemeName, null);
builder.Services.AddAuthorization(AuthorizationOptions);
builder.Services.AddHttpLogging(LoggingOptions);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEpcisStorage(builder.Configuration);
builder.Services.AddEpcisServices();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
builder.Services.Configure<Constants>(builder.Configuration.GetSection(nameof(Constants)));

// Handle persistent subscriptions in-memory.
// This will be enough for a single server deployment, but for a
// multi-instance setup it's better to externalize this process.
builder.Services.AddHostedService<SubscriptionBackgroundService>();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.MigrateDatabase();
}

app.UseDefaultFiles().UseStaticFiles();
app.UseRouting();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health").AllowAnonymous();
app.UseWebSockets();

app.AddCaptureEndpoints()
   .AddEventsEndpoints()
   .AddQueriesEndpoints()
   .AddSubscriptionEndpoints()
   .AddTopLevelEndpoints()
   .AddDiscoveryEndpoints();

// Map 1.x queries with SOAP service
app.AddSoapQueryService();

app.Run();

static void AuthorizationOptions(AuthorizationOptions options)
{
    options.AddPolicy("query", policy => policy.RequireClaim("fastnt.query"));
    options.AddPolicy("capture", policy => policy.RequireClaim("fastnt.capture"));
}

static void LoggingOptions(HttpLoggingOptions options)
{
    options.LoggingFields = HttpLoggingFields.All;
    options.MediaTypeOptions.AddText("application/xml");
    options.MediaTypeOptions.AddText("application/json");
    options.MediaTypeOptions.AddText("application/text+xml");
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
}