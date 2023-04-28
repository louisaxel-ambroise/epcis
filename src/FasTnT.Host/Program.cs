using FasTnT.Application;
using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Host.Features.v1_2;
using FasTnT.Host.Features.v2_0;
using FasTnT.Host.Services.Database;
using FasTnT.Host.Subscriptions;
using FasTnT.Host.Services.User;
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

// Handle persistent subscriptions in-memory.
// This will be enough for a single server deployment, but for a
// multi-instance setup it's better to externalize this process.
builder.Services.AddHostedService<SubscriptionBackgroundService>();

Constants.Instance = builder.Configuration.GetSection(nameof(Constants)).Get<Constants>();

var applyMigrations = builder.Configuration.GetValue("FasTnT.Database.ApplyMigrations", false);
var app = builder.Build();

app.MigrateDatabase(applyMigrations);
app.UseDefaultFiles().UseStaticFiles();
app.UseRouting();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health").AllowAnonymous();
app.UseWebSockets();
app.UseEpcis12Endpoints();
app.UseEpcis20Endpoints();

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