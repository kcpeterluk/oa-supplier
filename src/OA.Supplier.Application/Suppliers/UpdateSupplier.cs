using FluentValidation;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public static class UpdateSupplier
{
  public interface ICommandHandler
  {
    Task<bool> HandleAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default);
  }

  public class CommandHandler(ISupplierDomainService supplierDomainService, IValidator<UpdateSupplierRequest> validator) : ICommandHandler
  {
    public async Task<bool> HandleAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
      await validator.ValidateAndThrowAsync(request, cancellationToken);
      return await supplierDomainService.UpdateSupplier(request, cancellationToken);
    }
  }
}