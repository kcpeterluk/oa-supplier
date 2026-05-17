using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Domain.Suppliers;

public interface IGetSuppliersWithRatesQuery
{
  Task<IEnumerable<SupplierWithRatesProjection>> QueryAsync(CancellationToken cancellationToken = default);
}

public record SupplierWithRatesProjection(
  int Id,
  string Name,
  string Address,
  string CreatedByUser,
  DateTime CreatedOn,
  IReadOnlyCollection<SupplierRateProjection> SupplierRates);

