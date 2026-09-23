using BuildingBlocks.Shared.Exceptions.Handler;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var catalogAssembly = CatalogApplicationAssembly.Instance;
var basketAssembly = BasketApplicationAssembly.Instance;

builder.Services.AddCarterWithAssemblies(catalogAssembly, basketAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly, basketAssembly);

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


app.UseCatalogModule()
   .UseBasketModule();

app.Run();