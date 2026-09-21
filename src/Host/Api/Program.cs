var builder = WebApplication.CreateBuilder(args);

var catalogAssembly = CatalogApplicationAssembly.Instance;

builder.Services.AddCarterWithAssemblies(catalogAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly);

builder.Services.AddCatalogModule(builder.Configuration);


var app = builder.Build();

app.MapCarter();

//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();

//app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});


app.UseCatalogModule();



app.Run();