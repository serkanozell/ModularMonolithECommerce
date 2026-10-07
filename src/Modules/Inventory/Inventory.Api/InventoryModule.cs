using BuildingBlocks.Shared.Data;
using BuildingBlocks.Shared.Interceptors;
using Inventory.Domain.Repositories;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Inventory.Api
{
    public static class InventoryModule
    {
        public static IServiceCollection AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<InventoryDbContext>((serviceProvider, options) =>
            {
                options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(serviceProvider.GetRequiredService<DbConnection>())
                       .UseSnakeCaseNamingConvention();
            });

            return services;
        }

        public static IApplicationBuilder UseInventoryModule(this IApplicationBuilder app)
        {
            app.UseMigrations<InventoryDbContext>();
            return app;
        }
    }
}