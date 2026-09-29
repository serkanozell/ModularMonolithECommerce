using BuildingBlocks.Shared.Data;
using BuildingBlocks.Shared.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using System.Data.Common;

namespace Ordering.Api
{
    public static class OrderingModule
    {
        public static IServiceCollection AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
        {
            // MediatR, behavior ve validator kayıtları host tarafında
            // AddMediatRWithAssemblies ile merkezi olarak yapılır.

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.TryAddEnumerable(ServiceDescriptor.Scoped<ISaveChangesInterceptor, AuditableEntityInterceptor>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>());

            services.AddDbContext<OrderingDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(sp.GetRequiredService<DbConnection>())
                       .UseSnakeCaseNamingConvention();
            });

            return services;
        }

        public static IApplicationBuilder UseOrderingModule(this IApplicationBuilder app)
        {
            app.UseMigrations<OrderingDbContext>();

            return app;
        }
    }
}
