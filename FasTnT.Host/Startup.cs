using FasTnT.Application;
using FasTnT.Application.Queries.Poll;
using FasTnT.Application.Services;
using FasTnT.Host.v1_2;
using FasTnT.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace FasTnT.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IEpcisQuery, SimpleEventQuery>();
            services.AddDbContext<EpcisContext>(o => o.UseSqlServer("Server=(local);Database=EpcisEfCore;Integrated Security=true;").EnableSensitiveDataLogging());
            services.AddMediatR(Assembly.GetAssembly(typeof(ICommand<>)));

            var context = new EpcisContext(new DbContextOptionsBuilder<EpcisContext>().UseSqlServer("Server=(local);Database=EpcisEfCore;Integrated Security=true;").Options);
            context.Database.Migrate();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCaptureEpcis1_2("/v1_2/Capture.svc");
            app.UseQueryEpcis1_2("/v1_2/Query.svc");
        }
    }
}
