using FluentValidation;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Application.SupplierRates;

public static class UpdateSupplierRate
{
  public interface ICommandHandler
  {
    Task<bool> HandleAsync(UpdateSupplierRateRequest request, CancellationToken cancellationToken = default);
  }

  public class CommandHandler(ISupplierRateDomainService supplierRateDomainService, IValidator<UpdateSupplierRateRequest> validator) : ICommandHandler
  {
    public async Task<bool> HandleAsync(UpdateSupplierRateRequest request, CancellationToken cancellationToken = default)
    {
      await validator.ValidateAndThrowAsync(request, cancellationToken);
      return await supplierRateDomainService.UpdateSupplierRate(request, cancellationToken);
    }
  }
}
