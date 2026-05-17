using FluentValidation;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Application.SupplierRates;

public static class DeleteSupplierRate
{
  public record DeleteSupplierRateRequest(int Id);
  
  public interface ICommandHandler
  {
    Task<bool> HandleAsync(DeleteSupplierRateRequest request, CancellationToken cancellationToken = default);
  }

  public class CommandHandler(ISupplierRateDomainService supplierRateDomainService, IValidator<DeleteSupplierRateRequest> validator) : ICommandHandler
  {
    public async Task<bool> HandleAsync(DeleteSupplierRateRequest request, CancellationToken cancellationToken = default)
    {
      await validator.ValidateAndThrowAsync(request, cancellationToken);
      return await supplierRateDomainService.DeleteSupplierRate(request.Id, cancellationToken);
    }
  }
}
