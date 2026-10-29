using BuildingBlocks.Shared.Data;
using BuildingBlocks.Shared.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Payment.Domain.Repositories;
using Payment.Infrastructure.Persistance;
using Payment.Infrastructure.Repositories;
using System.Data.Common;

namespace Payment.Api
{
    public static class PaymentModule
    {
        public static IServiceCollection AddPaymentModule(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddOptions<PaymentOptions>().Bind(configuration.GetSection(PaymentOptions.SectionName));

            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            //services.AddScoped<IPaymentGateway, FakePaymentGateway>();

            services.TryAddEnumerable(ServiceDescriptor.Scoped<ISaveChangesInterceptor, AuditableEntityInterceptor>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>());

            services.AddDbContext<PaymentDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(sp.GetRequiredService<DbConnection>()).UseSnakeCaseNamingConvention();
            });

            return services;
        }

        public static IApplicationBuilder UsePaymentModule(this IApplicationBuilder app)
        {
            app.UseMigrations<PaymentDbContext>();
            return app;
        }
    }
}