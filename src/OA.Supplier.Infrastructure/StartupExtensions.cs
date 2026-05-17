using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Domain;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;
using OA.Supplier.Infrastructure.Persistence;
using OA.Supplier.Infrastructure.Persistence.SupplierRates;
using OA.Supplier.Infrastructure.Persistence.Suppliers;

namespace OA.Supplier.Infrastructure;

public static class StartupExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services, 
    IConfiguration configuration,
    bool isDevelopment = false)
  {
    string connectionString = GetConnectionString(configuration, isDevelopment);
    services.AddDbContext<SupplierDbContext>(options => options.UseSqlServer(connectionString));
    
    services.AddScoped<IRepository<Domain.Suppliers.Supplier>, SupplierRepository>();
    services.AddScoped<IRepository<SupplierRate>, SupplierRateRepository>();
    services.AddScoped<IGetSupplierRatesBySupplierQuery, GetSupplierRatesBySupplierQuery>();
    services.AddScoped<IGetSuppliersWithRatesQuery, GetSuppliersWithRatesQuery>();
    
    return services;
  }

  private static string GetConnectionString(IConfiguration configuration, bool isDevelopment)
  {
    SqlConnectionStringBuilder sqlConnectionStringBuilder = new(configuration.GetConnectionString("SupplierDbContext"))
    {
      TrustServerCertificate = true
    };
    
    if (isDevelopment)
    {
      sqlConnectionStringBuilder.UserID = Environment.GetEnvironmentVariable("MSSQL_SA_ID");
      sqlConnectionStringBuilder.Password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
    }
    
    string connectionString = sqlConnectionStringBuilder.ConnectionString;
    return connectionString;
  }
}
