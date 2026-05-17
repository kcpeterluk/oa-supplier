using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OA.Supplier.Infrastructure.Persistence;
using OA.Supplier.WebApp.Data.Identity;

namespace OA.Supplier.WebApp.Infrastructure.Persistence;

public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
{
  private readonly string _connectionString;

  public ApplicationIdentityDbContext (IDatabaseConnectionStringFactory databaseConnectionStringFactory)
  {
    _connectionString = databaseConnectionStringFactory.GetConnectionString();
  }
  
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlServer(_connectionString);
  }
}