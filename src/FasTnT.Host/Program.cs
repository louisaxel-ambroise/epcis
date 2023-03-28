using FasTnT.Application;
using FasTnT.Application.Services.Users;
using FasTnT.Host.Features.v1_2;
using FasTnT.Host.Features.v2_0;
using FasTnT.Host.Services.Database;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
Constants.Instance = builder.Configuration.GetSection(nameof(Constants)).Get<Constants>();

builder.Services.AddAuthentication(BasicAuthentication.SchemeName).AddScheme<AuthenticationSchemeOptions, BasicAuthentication>(BasicAuthentication.SchemeName, null);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("query", policy => policy.RequireClaim("fastnt.query"));
    options.AddPolicy("capture", policy => policy.RequireClaim("fastnt.capture"));
});
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.MediaTypeOptions.AddText("application/xml");
    options.MediaTypeOptions.AddText("application/json");
    options.MediaTypeOptions.AddText("application/text+xml");
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddEpcisStorage(builder.Configuration);
builder.Services.AddEpcisServices();
builder.Services.AddHostedService<SubscriptionBackgroundService>();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

var app = builder.Build();

if(builder.Configuration.GetValue("FasTnT.Database.ApplyMigrations", false))
{
    app.ApplyMigrations();
}

app.UseDefaultFiles().UseStaticFiles();
app.UseRouting();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health").AllowAnonymous();

app.UseEpcis12Endpoints();
app.UseEpcis20Endpoints();

app.Run();
