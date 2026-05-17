using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using OA.Supplier.WebApp.Components.Account;
using OA.Supplier.WebApp.Data.Identity;
using OA.Supplier.WebApp.Infrastructure.Persistence;

namespace OA.Supplier.WebApp.Infrastructure.Authentication;

public static class StartupExtensions
{
  public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
  {
    services.AddCascadingAuthenticationState();
    services.AddScoped<IdentityRedirectManager>();
    services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    services.AddAuthentication(options =>
      {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
      })
      .AddBearerToken(Constants.IdentityBearerScheme)
      .AddIdentityCookies();

    services.AddAuthorization();
    services.AddDbContext<ApplicationIdentityDbContext>();
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddIdentityCore<ApplicationUser>(options =>
      {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
      })
      .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
      .AddApiEndpoints()
      .AddSignInManager()
      .AddDefaultTokenProviders();

    services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
    
    return services;
  }
}
