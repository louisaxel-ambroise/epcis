using Carter;
using FasTnT.Application.Queries.Poll;
using FasTnT.Application.Services;
using FasTnT.Domain;
using FasTnT.Domain.Infrastructure.Behaviors;
using FasTnT.Infrastructure.Configuration;
using FasTnT.Infrastructure.Database;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FasTnT.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("FasTnT.Database");

            services.AddScoped<IdentityGenerator>();
            services.AddDbContext<EpcisContext>(o => o.UseSqlServer(connectionString));
            services.AddMediatR(typeof(PollQueryHandler).Assembly);
            services.AddValidatorsFromAssembly(typeof(CommandValidationBehavior<,>).Assembly);
            services.AddTransient<IEpcisQuery, SimpleEventQuery>();
            services.AddTransient<IEpcisQuery, SimpleMasterDataQuery>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));
            services.AddCarter(o => o.OpenApi.Enabled = true);

            var constantsSection = _configuration.GetSection("Constants");
            if (constantsSection.Exists())
            {
                Constants.MaxEventsReturnedInQuery = constantsSection.GetValue(nameof(Constants.MaxEventsReturnedInQuery), Constants.MaxEventsReturnedInQuery);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (_configuration.GetValue<bool>("FasTnT.Database.ApplyMigrations"))
            {
                using var scope = app.ApplicationServices.CreateScope();

                scope.ServiceProvider.GetService<EpcisContext>().Database.Migrate();
            }

            app.UseExceptionHandler("/epciserror");
            app.UseRouting();
            app.UseEndpoints(builder => builder.MapCarter());
        }
    }
}
