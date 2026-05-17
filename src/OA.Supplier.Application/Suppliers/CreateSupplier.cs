using FluentValidation;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public static class CreateSupplier
{
  public interface ICommandHandler
  {
    Task<SupplierDto> HandleAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
  }

  public class CommandHandler(ISupplierDomainService supplierDomainService, IValidator<CreateSupplierRequest> validator) : ICommandHandler
  {
    public async Task<SupplierDto> HandleAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
      await validator.ValidateAndThrowAsync(request, cancellationToken);
      Domain.Suppliers.Supplier supplier = await supplierDomainService.CreateSupplier(request, cancellationToken);
      return SupplierDto.MapFrom(supplier);
    }
  }
}