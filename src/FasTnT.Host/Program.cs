using FasTnT.Application;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Host.Features.v1_2;
using FasTnT.Host.Features.v2_0;
using FasTnT.Host.Services.Database;
using FasTnT.Host.Services.Notifications;
using FasTnT.Host.Services.Subscriptions;
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
builder.Services.AddHostedService<SubscriptionBackgroundService>();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

// Change this to another notification scheme if you intend to have multiple servers running.
// A queue-based notification pattern is probably recommended in that case.
// Also move the subscriptions handling in an other singletong process in that case.
builder.Services.AddSingleton<INotificationSender>(InMemoryNotificationManager.Instance);
builder.Services.AddSingleton<INotificationReceiver>(InMemoryNotificationManager.Instance);

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