using BuildingBlocks.Messaging.Behaviors;
using BuildingBlocks.Messaging.Persistence;
using BuildingBlocks.Messaging.Transactions;
using BuildingBlocks.Shared.Data;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data.Common;
using System.Reflection;

namespace BuildingBlocks.Messaging.Extensions
{
    public static class MessagingExtensions
    {
        public const string ConnectionStringName = "DefaultConnection";
        public const string MessageBrokerSectionName = "MessageBroker";

        /// <summary>
        /// Registers the shared connection, transaction infrastructure, MessagingDbContext (outbox/inbox)
        /// and MassTransit. Must be called after AddMediatRWithAssemblies so that TransactionBehavior
        /// runs innermost (after validation and logging).
        /// </summary>
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, params Assembly[] consumerAssemblies)
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' not found.");

            services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
            services.AddScoped<DbConnection>(sp => sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());

            services.AddScoped<ITransactionAccessor, TransactionAccessor>();
            services.AddScoped<EnlistTransactionInterceptor>();
            services.AddScoped<ISaveChangesInterceptor>(sp => sp.GetRequiredService<EnlistTransactionInterceptor>());

            services.AddDbContext<MessagingDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetRequiredService<EnlistTransactionInterceptor>());
                options.UseNpgsql(sp.GetRequiredService<DbConnection>());
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            var brokerOptions = configuration.GetSection(MessageBrokerSectionName).Get<MessageBrokerOptions>() ?? new MessageBrokerOptions();

            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                config.AddConsumers(consumerAssemblies);

                //config.AddSagaStateMachines(consumerAssemblies);

                //config.AddSagas(consumerAssemblies);

                //config.AddActivities(consumerAssemblies);

                config.AddEntityFrameworkOutbox<MessagingDbContext>(outbox =>
                {
                    outbox.UsePostgres();
                    outbox.UseBusOutbox();
                    outbox.QueryDelay = TimeSpan.FromSeconds(1);
                    outbox.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
                });

                config.AddConfigureEndpointsCallback((context, _, endpoint) =>
                {
                    endpoint.UseMessageRetry(retry => retry.Intervals(100, 500, 1000));
                    endpoint.UseEntityFrameworkOutbox<MessagingDbContext>(context);
                });
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(brokerOptions.Host, host =>
                    {
                        host.Username(brokerOptions.UserName);
                        host.Password(brokerOptions.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }

        public static IApplicationBuilder UseMessaging(this IApplicationBuilder app)
        {
            app.UseMigrations<MessagingDbContext>();

            return app;
        }
    }
}
