using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Domain.Suppliers;
using OA.Supplier.Infrastructure.Suppliers;

namespace OA.Supplier.Infrastructure;

public static class StartupExtensions
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services)
  {
    services.AddScoped<ISupplierRepository, SupplierRepository>();
    return services;
  }
}