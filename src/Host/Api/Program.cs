using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Shared.Caching;
using BuildingBlocks.Shared.Exceptions.Handler;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var catalogAssembly = CatalogApplicationAssembly.Instance;
var basketAssembly = BasketApplicationAssembly.Instance;


// extensiona taşınıp birbiri ile ilişkili şeyler methodlara bölünecek

builder.Services.AddCarterWithAssemblies(catalogAssembly, basketAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly, basketAssembly);

builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("RedisOptions"));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Must be registered after MediatR so that TransactionBehavior runs innermost
builder.Services.AddMessaging(builder.Configuration, catalogAssembly, basketAssembly);

builder.Services.AddCatalogModule(builder.Configuration)
                .AddBasketModule(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.MapCarter();

app.UseSerilogRequestLogging();

app.UseExceptionHandler(options => { });

//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();

//app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});


app.UseMessaging();

app.UseCatalogModule()
   .UseBasketModule();

app.Run();