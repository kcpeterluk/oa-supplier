using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OA.Supplier.Infrastructure.Persistence;
using OA.Supplier.WebApp.Infrastructure.Persistence;
using Testcontainers.MsSql;
using TUnit.Core.Interfaces;

namespace OA.Supplier.ComponentTests;

public class TestDatabaseWrapper : IAsyncInitializer, IAsyncDisposable
{
  private const string SqlImage = "mcr.microsoft.com/mssql/server:2025-latest";
  
  private MsSqlContainer? _mssqlContainer;

  public IDatabaseConnectionStringFactory DatabaseConnectionStringFactory { get; private set; } = null!;

  public async Task InitializeAsync()
  {
    _mssqlContainer = new MsSqlBuilder(SqlImage)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("SQL Server is now ready for client connections"))
      .Build();
    
    await _mssqlContainer.StartAsync();
    
    DatabaseConnectionStringFactory = new TestDatabaseDatabaseConnectionStringFactory(_mssqlContainer.GetConnectionString().Replace("127.0.0.1", "localhost"));
    
    SupplierDbContext supplierDbContext = new(DatabaseConnectionStringFactory);
    await supplierDbContext.Database.EnsureDeletedAsync();
    await supplierDbContext.Database.EnsureCreatedAsync();
        
    ApplicationIdentityDbContext applicationIdentityDbContext = new(DatabaseConnectionStringFactory);
    IRelationalDatabaseCreator applicationIdentityDbCreator = applicationIdentityDbContext.Database.GetService<IRelationalDatabaseCreator>();
    await applicationIdentityDbCreator.CreateTablesAsync();
  }

  public async ValueTask DisposeAsync()
  {
    if (_mssqlContainer is not null)
    {
      SupplierDbContext supplierDbContext = new(DatabaseConnectionStringFactory);
      await supplierDbContext.Database.EnsureDeletedAsync();
      await _mssqlContainer.DisposeAsync();
    }
  }
}