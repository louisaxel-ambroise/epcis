using FasTnT.Application.EfCore;
using FasTnT.Domain;
using FasTnT.Features.v1_2;
using FasTnT.Features.v1_2.Communication;
using FasTnT.Features.v2_0;
using FasTnT.Features.v2_0.Communication.Json;
using FasTnT.Host.Extensions;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Constants.Instance = builder.Configuration.GetSection(nameof(Constants)).Get<Constants>();

builder.Services.AddAuthentication(BasicAuthenticationHandler.SchemeName).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization(Options.AuthorizationPolicies);
builder.Services.AddHttpLogging(Options.LoggingPolicy);
builder.Services.AddHttpContextAccessor();

// Add the subscription manager as background service
builder.Services.AddEpcisSubscriptionServices(XmlResultSender.Instance, JsonResultSender.Instance);
builder.Services.AddHostedService<SubscriptionBackgroundService>();

builder.Services.AddEpcisServices(opt =>
{
    opt.ConnectionString = builder.Configuration.GetConnectionString("FasTnT.Database");
    opt.CommandTimeout = builder.Configuration.GetValue("FasTnT.Database.ConnectionTimeout", 60);
    opt.CurrentUser = svc => new HttpContextCurrentUser(svc.GetRequiredService<IHttpContextAccessor>());
});

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

app.UseEpcis12Endpoints();
app.UseEpcis20Endpoints();

app.Run();
