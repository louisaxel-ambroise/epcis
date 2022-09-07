using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Host.Authorization;
using FasTnT.Host.Features.v1_2;
using FasTnT.Host.Features.v2_0;
using FasTnT.Infrastructure.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FasTnT.Database");
var commandTimeout = builder.Configuration.GetValue("FasTnT.Database.ConnectionTimeout", 60);
var constantsSection = builder.Configuration.GetSection(nameof(Constants));
Constants.MaxEventsReturnedInQuery = constantsSection.GetValue(nameof(Constants.MaxEventsReturnedInQuery), Constants.MaxEventsReturnedInQuery);

builder.Services.AddAuthentication(BasicAuthenticationHandler.SchemeName).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization(Options.AuthorizationPolicies);
builder.Services.AddHttpLogging(Options.LoggingPolicy);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSqlServer<EpcisContext>(connectionString, opt => opt.MigrationsAssembly("FasTnT.Migrations.SqlServer").EnableRetryOnFailure().CommandTimeout(commandTimeout));

// Register EPCIS 1.2 services
builder.Services.AddEpcis12Services();
builder.Services.AddEpcis12SubscriptionService();
builder.Services.AddScoped<IUserProvider>(svc => builder.Environment.IsDevelopment() ? new DefaultUserProvider(svc.GetService<EpcisContext>()) : new UserProvider(svc.GetService<EpcisContext>()));

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDefaultFiles()
       .UseStaticFiles();
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
