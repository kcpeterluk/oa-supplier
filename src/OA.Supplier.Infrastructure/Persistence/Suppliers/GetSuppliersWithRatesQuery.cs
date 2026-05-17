using Microsoft.EntityFrameworkCore;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Infrastructure.Persistence.Suppliers;

internal class GetSuppliersWithRatesQuery(SupplierDbContext supplierDbContext) : IGetSuppliersWithRatesQuery
{
  public async Task<IEnumerable<SupplierWithRatesProjection>> QueryAsync(CancellationToken cancellationToken = default)
  {
    return await supplierDbContext.Supplier
      .AsNoTracking()
      .AsSplitQuery()
      .OrderBy(supplier => supplier.Id)
      .Select(supplier => new SupplierWithRatesProjection(
        supplier.Id,
        supplier.Name,
        supplier.Address,
        supplier.CreatedByUser,
        supplier.CreatedOn,
        supplierDbContext.SupplierRate
          .Where(supplierRate => supplierRate.SupplierId == supplier.Id)
          .OrderBy(supplierRate => supplierRate.RateStartDate)
          .Select(supplierRate => new SupplierRateProjection(
            supplierRate.Id,
            supplierRate.SupplierId,
            supplierRate.Rate,
            supplierRate.RateStartDate,
            supplierRate.RateEndDate,
            supplierRate.CreatedByUser,
            supplierRate.CreatedOn))
          .ToArray()))
      .ToArrayAsync(cancellationToken);
  }
}
