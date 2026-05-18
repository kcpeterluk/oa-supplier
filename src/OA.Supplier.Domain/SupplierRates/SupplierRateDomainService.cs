namespace OA.Supplier.Domain.SupplierRates;

public interface ISupplierRateDomainService
{
  Task<SupplierRate> CreateSupplierRate(CreateSupplierRateRequest request, CancellationToken cancellationToken = default);

  Task<bool> UpdateSupplierRate(UpdateSupplierRateRequest request, CancellationToken cancellationToken = default);

  Task<bool> DeleteSupplierRate(int id, CancellationToken cancellationToken = default);
}

public class SupplierRateDomainService(IRepository<SupplierRate> supplierRateRepository) : ISupplierRateDomainService
{
  public async Task<SupplierRate> CreateSupplierRate(CreateSupplierRateRequest request, CancellationToken cancellationToken = default)
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

  public async Task<bool> UpdateSupplierRate(UpdateSupplierRateRequest request, CancellationToken cancellationToken = default)
  {
    SupplierRate? supplierRate = await supplierRateRepository.GetByIdAsync(request.Id, cancellationToken);

    if (supplierRate is null)
    {
      throw new InvalidOperationException("Supplier rate not found");
    }

    if (supplierRate.SupplierId != request.SupplierId)
    {
      throw new InvalidOperationException("Supplier rate not found");
    }

    supplierRate.Update(request.Rate, request.RateStartDate, request.RateEndDate);

    return await supplierRateRepository.UpdateAsync(supplierRate, cancellationToken);
  }

  public Task<bool> DeleteSupplierRate(int id, CancellationToken cancellationToken = default) =>
    supplierRateRepository.DeleteAsync(id, cancellationToken);
}
