using Microsoft.Data.SqlClient;
using OA.Supplier.Infrastructure.Persistence;

namespace OA.Supplier.ComponentTests;

public class TestDatabaseDatabaseConnectionStringFactory(string connectionString) : IDatabaseConnectionStringFactory
{
  public string GetConnectionString()
  {
    SqlConnectionStringBuilder builder = new(connectionString)
    {
      InitialCatalog = "SupplierComponentTestsDb"
    };
    
    return builder.ConnectionString;
  }
}