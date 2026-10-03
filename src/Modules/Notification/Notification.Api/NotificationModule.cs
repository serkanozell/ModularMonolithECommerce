using BuildingBlocks.Shared.Data;
using BuildingBlocks.Shared.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Abstractions;
using Notification.Application.Options;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Email;
using Notification.Infrastructure.Persistence;
using Notification.Infrastructure.Repositories;
using Notification.Infrastructure.Sms;
using System.Data.Common;

namespace Notification.Api
{
    public static class NotificationModule
    {
        public static IServiceCollection AddNotificationModule(this IServiceCollection services, IConfiguration configuration)
        {
            // MediatR, behavior ve validator kayıtları host tarafında
            // AddMediatRWithAssemblies ile merkezi olarak yapılır.

            // validate on start için fluentvalidation doğrulaması yapılmalı
            services.AddOptions<OtpOptions>()
                    .Bind(configuration.GetSection(OtpOptions.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddOptions<SmtpOptions>()
                    .Bind(configuration.GetSection(SmtpOptions.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<ISmsSender, FakeSmsSender>();

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<NotificationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(sp.GetRequiredService<DbConnection>())
                       .UseSnakeCaseNamingConvention();
            });

            return services;
        }

        public static IApplicationBuilder UseNotificationModule(this IApplicationBuilder app)
        {
            app.UseMigrations<NotificationDbContext>();

            return app;
        }
    }
}
