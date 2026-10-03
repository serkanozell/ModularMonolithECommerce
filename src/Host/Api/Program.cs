using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Shared.Caching;
using BuildingBlocks.Shared.Exceptions.Handler;
using BuildingBlocks.Shared.Services;
using Notification.Api;
using Notification.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var catalogAssembly = CatalogApplicationAssembly.Instance;
var basketAssembly = BasketApplicationAssembly.Instance;
var orderingAssembly = OrderingApplicationAssembly.Instance;
var notificationAssembly = NotificationApplicationAssembly.Instance;


// extensiona taşınıp birbiri ile ilişkili şeyler methodlara bölünecek

builder.Services.AddCarterWithAssemblies(catalogAssembly, basketAssembly, orderingAssembly, notificationAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly, basketAssembly, orderingAssembly, notificationAssembly);

builder.Services.AddOptions<RedisOptions>()
                .Bind(builder.Configuration.GetSection("RedisOptions"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddOptions<MessageBrokerOptions>()
                .Bind(builder.Configuration.GetSection("MessageBroker"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Must be registered after MediatR so that TransactionBehavior runs innermost
builder.Services.AddMessaging(builder.Configuration, catalogAssembly, basketAssembly, orderingAssembly, notificationAssembly);

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, "Keycloak");

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddAuthorization();

builder.Services.AddCatalogModule(builder.Configuration)
                .AddBasketModule(builder.Configuration)
                .AddOrderingModule(builder.Configuration)
                .AddNotificationModule(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.MapCarter();

app.UseSerilogRequestLogging();

app.UseExceptionHandler(options => { });

//app.UseStaticFiles();

//app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});


app.UseMessaging();

app.UseCatalogModule()
   .UseBasketModule()
   .UseOrderingModule()
   .UseNotificationModule();

app.Run();