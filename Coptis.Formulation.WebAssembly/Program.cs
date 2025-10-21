using Coptis.Formulation.WebAssembly;
using Coptis.Formulation.WebAssembly.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7127/")
});

// Register API Clients
builder.Services.AddScoped<FormulasApiClient>();
builder.Services.AddScoped<RawMaterialsApiClient>();
builder.Services.AddScoped<SubstancesApiClient>();

await builder.Build().RunAsync();