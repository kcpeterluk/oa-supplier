using Microsoft.Extensions.Logging;

namespace OA.Supplier.Domain.SupplierRates;

public interface ISupplierRateDomainService
{
  Task<SupplierRate> CreateSupplierRate(CreateSupplierRateRequest request, CancellationToken cancellationToken = default);
}

public class SupplierRateDomainService(IRepository<SupplierRate> supplierRateRepository, ILogger<SupplierRateDomainService> logger) : ISupplierRateDomainService
{
  public async Task<SupplierRate> CreateSupplierRate(CreateSupplierRateRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      SupplierRate supplierRate = SupplierRate.Create(
        request.SupplierId,
        request.Rate,
        request.RateStartDate,
        request.RateEndDate,
        request.CreatedByUser);

      await supplierRateRepository.AddAsync(supplierRate, cancellationToken);

      return supplierRate;
    }
    catch (Exception e)
    {
      logger.LogError(e, "Error creating supplier rate");
      throw;
    }
  }
}
