using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application;

public static class StartupExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<CreateSupplier.ICommandHandler, CreateSupplier.CommandHandler>();
    services.AddScoped<IValidator<CreateSupplierRequest>, CreateSupplierRequestValidator>();

    services.AddScoped<GetSuppliersQueryHandler.IQueryHandler, GetSuppliersQueryHandler.QueryHandler>();
    
    services.AddScoped<UpdateSupplier.ICommandHandler, UpdateSupplier.CommandHandler>();
    services.AddScoped<IValidator<UpdateSupplierRequest>, UpdateSupplierRequestValidator>();
    
    services.AddScoped<DeleteSupplier.ICommandHandler, DeleteSupplier.CommandHandler>();
    services.AddScoped<IValidator<DeleteSupplier.DeleteSupplierRequest>, DeleteSupplierRequestValidator>();
    
    
    return services;
  }
}
