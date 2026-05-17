using Microsoft.EntityFrameworkCore;

namespace OA.Supplier.Infrastructure.Persistence;

public class SupplierDbContext : DbContext
{
  private readonly string _connectionString;

  public SupplierDbContext (IConnectionStringFactory connectionStringFactory)
  {
    _connectionString = connectionStringFactory.GetConnectionString();
  }
  
  public DbSet<Domain.Suppliers.Supplier> Supplier { get; set; }
  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema("supplier");
    modelBuilder.UseNamedDefaultConstraints();
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupplierDbContext).Assembly);
  }
  
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlServer(_connectionString);
  }
}

public interface IConnectionStringFactory
{
  string GetConnectionString();
}