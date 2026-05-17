using Microsoft.EntityFrameworkCore;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Infrastructure.Persistence.SupplierRates;

internal class GetSupplierRatesBySupplierQuery(SupplierDbContext supplierDbContext) : IGetSupplierRatesBySupplierQuery
{
  public async Task<IEnumerable<SupplierRate>> QueryAsync(int supplierId, CancellationToken cancellationToken = default)
  {
    return await supplierDbContext.SupplierRate
      .Where(supplierRate => supplierRate.SupplierId == supplierId)
      .OrderBy(supplierRate => supplierRate.RateStartDate)
      .AsNoTracking()
      .ToArrayAsync(cancellationToken);
  }
}