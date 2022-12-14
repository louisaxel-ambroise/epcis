using FasTnT.Application;
using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Host.Features.v1_2;
using FasTnT.Host.Features.v1_2.Subscriptions;
using FasTnT.Host.Features.v2_0;
using FasTnT.Host.Features.v2_0.Subscriptions;
using FasTnT.Host.Services.Database;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
Constants.Instance = builder.Configuration.GetSection(nameof(Constants)).Get<Constants>();

builder.Services.AddAuthentication(BasicAuthenticationHandler.SchemeName).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization(Options.AuthorizationPolicies);
builder.Services.AddHttpLogging(Options.LoggingPolicy);
builder.Services.AddHttpContextAccessor();

// Add the subscription manager as background service
builder.Services.AddEpcisSubscriptionServices(XmlResultSender.Instance, JsonResultSender.Instance);
builder.Services.AddHostedService<SubscriptionBackgroundService>();

builder.Services.AddScoped<ICurrentUser>(svc => new HttpContextCurrentUser(svc.GetRequiredService<IHttpContextAccessor>()));
builder.Services.AddEpcisStorage(builder.Configuration);
builder.Services.AddEpcisServices();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDefaultFiles().UseStaticFiles();
}
if(builder.Configuration.GetValue("FasTnT.Database.ApplyMigrations", false))
{
    app.ApplyMigrations();
}

app.UseRouting();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.UseWebSockets();

app.UseEpcis12Endpoints();
app.UseEpcis20Endpoints();

app.Run();
