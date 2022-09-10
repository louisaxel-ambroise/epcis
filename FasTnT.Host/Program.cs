using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Host.Authorization;
using FasTnT.Host.Extensions;
using FasTnT.Host.Features.v1_2;
using FasTnT.Host.Features.v2_0;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using FasTnT.Infrastructure.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Constants.MaxEventsReturnedInQuery = builder.Configuration.GetSection(nameof(Constants)).GetValue(nameof(Constants.MaxEventsReturnedInQuery), Constants.MaxEventsReturnedInQuery);

builder.Services.AddAuthentication(BasicAuthenticationHandler.SchemeName).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization(Options.AuthorizationPolicies);
builder.Services.AddHttpLogging(Options.LoggingPolicy);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEpcis12SubscriptionService<SubscriptionBackgroundService>(); // Register EPCIS 1.2 subscription services
builder.Services.AddEpcisServices(opt =>
{
    opt.ConnectionString = builder.Configuration.GetConnectionString("FasTnT.Database");
    opt.CommandTimeout = builder.Configuration.GetValue("FasTnT.Database.ConnectionTimeout", 60);
    opt.CurrentUser = svc => new HttpContextCurrentUser(svc.GetRequiredService<IHttpContextAccessor>());
    opt.UserProvider = svc => builder.Environment.IsDevelopment() ? new DefaultUserProvider(svc.GetService<EpcisContext>()) : new UserProvider(svc.GetService<EpcisContext>());
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
app.UseExceptionHandler(Options.ExceptionHandler);
app.UseEndpoints(builder =>
{
    builder.MapEpcis12Endpoints();
    builder.MapEpcis20Endpoints();
});

app.Run();
