using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OA.Supplier.Infrastructure.Persistence;
using OA.Supplier.WebApp.Data.Identity;
using TUnit.AspNetCore;

namespace OA.Supplier.ComponentTests;

public class WebApplicationFactory : TestWebApplicationFactory<Program>
{
  private const string TestUserEmail = "supplier-api-user@example.com";
  private const string TestUserPassword = "SupplierTest1!";
  
  private readonly SemaphoreSlim _accessTokenSemaphore = new(1, 1);
  private string? _accessToken;
  
  [ClassDataSource<TestDatabaseWrapper>(Shared = SharedType.PerTestSession)]
  public required TestDatabaseWrapper Database { get; init; } = null!;

  public async Task<string> GetAccessTokenAsync()
  {
    if (_accessToken is not null)
    {
      return _accessToken;
    }

    await _accessTokenSemaphore.WaitAsync();
    try
    {
      if (_accessToken is not null)
      {
        return _accessToken;
      }

      await EnsureTestUserExistsAsync();

      using HttpClient client = CreateClient(new WebApplicationFactoryClientOptions
      {
        AllowAutoRedirect = false
      });

      using HttpResponseMessage response = await client.PostAsJsonAsync("/api/account/login?useCookies=false", new
      {
        email = TestUserEmail,
        password = TestUserPassword
      });

      if (!response.IsSuccessStatusCode)
      {
        string responseBody = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Failed to sign in the supplier API test user. Status: {response.StatusCode}. Body: {responseBody}");
      }

      LoginResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
      _accessToken = loginResponse?.AccessToken ?? throw new InvalidOperationException("The supplier API test login response did not include an access token.");

      return _accessToken;
    }
    finally
    {
      _accessTokenSemaphore.Release();
    }
  }
  
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      builder.UseContentRoot(Directory.GetCurrentDirectory());
      
      services.RemoveAll<IDatabaseConnectionStringFactory>();
      services.AddSingleton<IDatabaseConnectionStringFactory>(_ => Database.DatabaseConnectionStringFactory);
    });
  }

  private async Task EnsureTestUserExistsAsync()
  {
    using IServiceScope scope = Services.CreateScope();
    UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    ApplicationUser? user = await userManager.FindByEmailAsync(TestUserEmail);
    if (user is null)
    {
      user = new ApplicationUser
      {
        UserName = TestUserEmail,
        Email = TestUserEmail,
        EmailConfirmed = true
      };

      IdentityResult createResult = await userManager.CreateAsync(user, TestUserPassword);
      if (!createResult.Succeeded)
      {
        string errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Failed to create the supplier API test user. {errors}");
      }

      return;
    }

    if (!user.EmailConfirmed)
    {
      user.EmailConfirmed = true;
      IdentityResult updateResult = await userManager.UpdateAsync(user);
      if (!updateResult.Succeeded)
      {
        string errors = string.Join(", ", updateResult.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Failed to confirm the supplier API test user. {errors}");
      }
    }
  }

  private sealed record LoginResponse(string AccessToken);
}
