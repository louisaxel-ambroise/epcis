using FasTnT.Application.EfCore;
using FasTnT.Application.EfCore.Services.Users;
using FasTnT.Application.EfCore.Store;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain;
using FasTnT.Features.v1_2;
using FasTnT.Features.v1_2.Communication;
using FasTnT.Features.v2_0;
using FasTnT.Features.v2_0.Communication.Json;
using FasTnT.Host.Authorization;
using FasTnT.Host.Extensions;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Constants.MaxEventsReturnedInQuery = builder.Configuration.GetSection(nameof(Constants)).GetValue(nameof(Constants.MaxEventsReturnedInQuery), Constants.MaxEventsReturnedInQuery);

builder.Services.AddAuthentication(BasicAuthenticationHandler.SchemeName).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization(Options.AuthorizationPolicies);
builder.Services.AddHttpLogging(Options.LoggingPolicy);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEpcisServices(opt =>
{
    opt.ConnectionString = builder.Configuration.GetConnectionString("FasTnT.Database");
    opt.CommandTimeout = builder.Configuration.GetValue("FasTnT.Database.ConnectionTimeout", 60);
    opt.CurrentUser = svc => new HttpContextCurrentUser(svc.GetRequiredService<IHttpContextAccessor>());
    opt.UserProvider = svc => new DefaultUserProvider(svc.GetService<EpcisContext>());
});

// Add the subscription manager as background service
builder.Services.AddEpcisSubscriptionServices(XmlResultSender.Instance, JsonResultSender.Instance);
builder.Services.AddSingleton<IResultSender>(XmlResultSender.Instance);
builder.Services.AddSingleton<IResultSender>(JsonResultSender.Instance);
builder.Services.AddHostedService<SubscriptionBackgroundService>();

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
app.UseExceptionHandler(Options.ExceptionHandler);
app.UseEndpoints(builder =>
{
    builder.MapEpcis12Endpoints();
    builder.MapEpcis20Endpoints();
});

app.Run();
