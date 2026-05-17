namespace OA.Supplier.Domain.SupplierRates;

public interface IGetSupplierRatesBySupplierQuery
{
  Task<IEnumerable<SupplierRate>> QueryAsync(int supplierId, CancellationToken cancellationToken = default);
}