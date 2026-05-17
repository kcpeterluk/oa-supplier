using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public static class GetSuppliersWithRatesQueryHandler
{
  public interface IQueryHandler
  {
    Task<IEnumerable<SupplierWithRatesDto>> HandleAsync(CancellationToken cancellationToken = default);
  }

  public class QueryHandler(IGetSuppliersWithRatesQuery query) : IQueryHandler
  {
    public async Task<IEnumerable<SupplierWithRatesDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
      IEnumerable<SupplierWithRatesProjection> suppliers = await query.QueryAsync(cancellationToken);
      return suppliers.Select(MapFrom);
    }

    private static SupplierWithRatesDto MapFrom(SupplierWithRatesProjection supplier)
    {
      return new SupplierWithRatesDto(
        supplier.Id,
        supplier.Name,
        supplier.Address,
        supplier.CreatedByUser,
        supplier.CreatedOn,
        supplier.SupplierRates.Select(supplierRate => new SupplierRateDto(
          supplierRate.Id,
          supplierRate.SupplierId,
          supplierRate.Rate,
          supplierRate.RateStartDate,
          supplierRate.RateEndDate,
          supplierRate.CreatedByUser,
          supplierRate.CreatedOn)));
    }
  }
}

