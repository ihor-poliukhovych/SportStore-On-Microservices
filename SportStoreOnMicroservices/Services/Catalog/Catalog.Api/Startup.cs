﻿using Catalog.Api.Infrastructure.Authentication;
using Catalog.Api.Infrastructure.Swagger;
using Catalog.Api.Persistence.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog.Api
{
    public class Startup
    {
        private const string SwaggerPageName = "Catalog Api";
        
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAuthOptionProvider, AuthOptionProvider>();    
            
            var serviceProvider = services.BuildServiceProvider();
            var authOptionProvider = serviceProvider.GetService<IAuthOptionProvider>();
            var connectionString = _configuration.GetConnectionString("CatalogDbConnection");
            
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            
            services.AddSportStoreSystemAuthentication(authOptionProvider.GetSystemAuthOptions());
            services.AddDbContext<CatalogDbContext>(x => x.UseSqlServer(connectionString));
            services.AddSwagger(SwaggerPageName);          
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseAuthentication();
            app.UseSwaggerUI(SwaggerPageName);
            app.UseMvcWithDefaultRoute();
        }
    }
}