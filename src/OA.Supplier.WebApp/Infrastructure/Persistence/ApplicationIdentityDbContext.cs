using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OA.Supplier.Infrastructure.Persistence;
using OA.Supplier.WebApp.Data.Identity;

namespace OA.Supplier.WebApp.Infrastructure.Persistence;

public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
{
  private readonly string _connectionString;

  public ApplicationIdentityDbContext (IConnectionStringFactory connectionStringFactory)
  {
    _connectionString = connectionStringFactory.GetConnectionString();
  }
  
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlServer(_connectionString);
  }
}