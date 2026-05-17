namespace OA.Supplier.Infrastructure.Persistence;

public interface IDatabaseConnectionStringFactory
{
  string GetConnectionString();
}