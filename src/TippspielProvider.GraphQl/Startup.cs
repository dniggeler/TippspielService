using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using MatchProvider;
using MatchProvider.Contracts.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TippspielProvider.GraphQl.Types.GraphQl;

namespace TippspielProvider.GraphQl
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddConnectors();

            services.AddGraphQL(c =>
            {
                c.Options.DeveloperMode = true;
                c.Options.ExecutionTimeout = TimeSpan.FromMinutes(5);
                c.RegisterQueryType<QueryType>();
            });
            //services.AddGraphQL(c => c.RegisterType<MatchDataModelType>());
            
            services.Configure<MatchProviderSettings>(_configuration.GetSection("MatchProvider"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseGraphQL();


            // fallback for root GET
            app.Run(context =>
            {
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsync($"OK - {DateTimeOffset.Now}, {context.Session.Id}");
            });
        }
    }
}
