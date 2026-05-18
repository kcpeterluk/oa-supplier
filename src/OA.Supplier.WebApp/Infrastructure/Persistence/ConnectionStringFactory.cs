using Microsoft.Data.SqlClient;
using OA.Supplier.Infrastructure.Persistence;

namespace OA.Supplier.WebApp.Infrastructure.Persistence;

public class DatabaseConnectionStringFactory : IDatabaseConnectionStringFactory
{
  private readonly string _connectionString;
  
  public DatabaseConnectionStringFactory(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
  {
    SqlConnectionStringBuilder sqlConnectionStringBuilder = new(configuration.GetConnectionString("SupplierDbContext"))
    {
      TrustServerCertificate = true
    };
    
    if (webHostEnvironment.IsDevelopment() && !sqlConnectionStringBuilder.IntegratedSecurity)
    {
      sqlConnectionStringBuilder.UserID = Environment.GetEnvironmentVariable("MSSQL_SA_ID");
      sqlConnectionStringBuilder.Password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
    }
    
    _connectionString = sqlConnectionStringBuilder.ConnectionString;
  }
  
  public string GetConnectionString() => _connectionString;
}