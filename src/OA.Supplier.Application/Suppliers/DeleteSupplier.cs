using FluentValidation;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public static class DeleteSupplier
{
  public record DeleteSupplierRequest(int Id);
  
  public interface ICommandHandler
  {
    Task<bool> HandleAsync(DeleteSupplierRequest request, CancellationToken cancellationToken = default);
  }

  public class CommandHandler(ISupplierDomainService supplierDomainService, IValidator<DeleteSupplierRequest> validator) : ICommandHandler
  {
    public async Task<bool> HandleAsync(DeleteSupplierRequest request, CancellationToken cancellationToken = default)
    {
      await validator.ValidateAndThrowAsync(request, cancellationToken);
      return await supplierDomainService.DeleteSupplier(request.Id, cancellationToken);
    }
  }
}