using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiniGameCollection.Web;
using MiniGameCollection.Web.Services;
using MiniGameCollection.Web.Storage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register game services
builder.Services.AddScoped<IBrowserStorage, BrowserStorage>();
builder.Services.AddScoped<IGameStateService, GameStateService>();

await builder.Build().RunAsync();
