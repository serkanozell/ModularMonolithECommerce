using BuildingBlocks.Shared.Data;
using BuildingBlocks.Shared.Interceptors;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Catalog.Api
{
    public static class CatalogModule
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register services, repositories, and other dependencies here
            // Example:
            // services.AddScoped<ICatalogService, CatalogService>();
            // services.AddScoped<ICatalogRepository, CatalogRepository>();

            // MediatR, behavior ve validator kayıtları host tarafında
            // AddMediatRWithAssemblies ile merkezi olarak yapılır.

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<CatalogDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(sp.GetRequiredService<DbConnection>());
            });

            return services;
        }

        public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
        {
            // Configure middleware, routing, etc. here
            // Example:
            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseMigrations<CatalogDbContext>();

            return app;
        }
    }
}