using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application;

public static class StartupExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddValidatorsFromAssemblyContaining<CreateSupplierRequestValidator>();
    
    services.AddScoped<CreateSupplier.ICommandHandler, CreateSupplier.CommandHandler>();
    services.AddScoped<GetSuppliersQueryHandler.IQueryHandler, GetSuppliersQueryHandler.QueryHandler>();
    services.AddScoped<GetSuppliersWithRatesQueryHandler.IQueryHandler, GetSuppliersWithRatesQueryHandler.QueryHandler>();
    services.AddScoped<GetOverlappingSuppliersWithRatesQueryHandler.IQueryHandler, GetOverlappingSuppliersWithRatesQueryHandler.QueryHandler>();
    services.AddScoped<GetSupplierQueryHandler.IQueryHandler, GetSupplierQueryHandler.QueryHandler>();
    services.AddScoped<UpdateSupplier.ICommandHandler, UpdateSupplier.CommandHandler>();
    services.AddScoped<DeleteSupplier.ICommandHandler, DeleteSupplier.CommandHandler>();
    
    services.AddScoped<GetSupplierRatesQueryHandler.IQueryHandler, GetSupplierRatesQueryHandler.QueryHandler>();
    services.AddScoped<CreateSupplierRate.ICommandHandler, CreateSupplierRate.CommandHandler>();
    services.AddScoped<UpdateSupplierRate.ICommandHandler, UpdateSupplierRate.CommandHandler>();
    services.AddScoped<DeleteSupplierRate.ICommandHandler, DeleteSupplierRate.CommandHandler>();
    
    return services;
  }
  
  public static IServiceCollection AddDomainServices(this IServiceCollection services)
  {
    services.AddScoped<ISupplierDomainService, SupplierDomainService>();
    services.AddScoped<ISupplierRateDomainService, SupplierRateDomainService>();
    return services;
  }
}
