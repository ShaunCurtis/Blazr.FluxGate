global using Blazr.FluxGate;
global using Microsoft.AspNetCore.Components.QuickGrid;

using Blazr.FluxGate.Server;
using Blazr.FluxGate.Server.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<FluxGateStore<CounterState>>();
builder.Services.AddSingleton<FluxGateDispatcher<CounterState>, CounterStateDispatcher>();

builder.Services.AddScoped<KeyedFluxGateStoreCollection<GridState, Guid>>();
builder.Services.AddSingleton<FluxGateDispatcher<GridState>, GridStateDispatcher>();

builder.Services.AddSingleton<WeatherProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
