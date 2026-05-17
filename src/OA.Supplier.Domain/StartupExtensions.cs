using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Domain;

public static class StartupExtensions
{
  public static IServiceCollection AddDomainServices(this IServiceCollection services)
  {
    services.AddScoped<ISupplierDomainService, SupplierDomainService>();
    return services;
  }
}