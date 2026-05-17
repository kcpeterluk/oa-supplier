using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using OA.Supplier.Application;
using OA.Supplier.Domain;
using OA.Supplier.Infrastructure;
using OA.Supplier.Infrastructure.Persistence;
using OA.Supplier.WebApp;
using OA.Supplier.WebApp.Apis.Suppliers;
using OA.Supplier.WebApp.Components;
using OA.Supplier.WebApp.Components.Account;
using OA.Supplier.WebApp.Data.Identity;
using OA.Supplier.WebApp.Infrastructure.Authentication;
using OA.Supplier.WebApp.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthenticationServices();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IDatabaseConnectionStringFactory, DatabaseConnectionStringFactory>();
builder.Services
    .AddApplicationServices()
    .AddDomainServices()
    .AddInfrastructureServices(builder.Configuration);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
app.MapGroup("/api/account").MapIdentityApi<ApplicationUser>();

app.MapSupplierEndpoints();

app.Run();
