using Basket.Domain.Repositories;
using Basket.Infrastructure.Persistence;
using Basket.Infrastructure.Repositories;
using BuildingBlocks.Shared.Data;
using BuildingBlocks.Shared.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Basket.Api
{
    public static class BasketModule
    {
        public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
        {
            // MediatR, behavior ve validator kayıtları host tarafında
            // AddMediatRWithAssemblies ile merkezi olarak yapılır.

            services.AddScoped<IBasketRepository, BasketRepository>();

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<BasketDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(sp.GetRequiredService<DbConnection>());
            });

            return services;
        }

        public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
        {
            app.UseMigrations<BasketDbContext>();

            return app;
        }
    }
}
