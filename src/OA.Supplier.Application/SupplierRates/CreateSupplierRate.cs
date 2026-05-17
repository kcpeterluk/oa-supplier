using FluentValidation;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Application.SupplierRates;

public static class CreateSupplierRate
{
  public interface ICommandHandler
  {
    Task<SupplierRateDto> HandleAsync(CreateSupplierRateRequest request, CancellationToken cancellationToken = default);
  }

  public class CommandHandler(ISupplierRateDomainService supplierRateDomainService, IValidator<CreateSupplierRateRequest> validator) : ICommandHandler
  {
    public async Task<SupplierRateDto> HandleAsync(CreateSupplierRateRequest request, CancellationToken cancellationToken = default)
    {
      await validator.ValidateAndThrowAsync(request, cancellationToken);
      SupplierRate supplierRate = await supplierRateDomainService.CreateSupplierRate(request, cancellationToken);
      return SupplierRateDto.MapFrom(supplierRate);
    }
  }
}
