using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OA.Supplier.Infrastructure.Persistence;

namespace OA.Supplier.ComponentTests;

public class WebApplicationFactory : WebApplicationFactory<Program>
{
  [ClassDataSource<TestDatabaseWrapper>(Shared = SharedType.PerTestSession)]
  public required TestDatabaseWrapper Database { get; init; } = null!;
  
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      builder.UseContentRoot(Directory.GetCurrentDirectory());
      
      services.RemoveAll<IDatabaseConnectionStringFactory>();
      services.AddSingleton<IDatabaseConnectionStringFactory>(_ => Database.DatabaseConnectionStringFactory);
    });
  }
}