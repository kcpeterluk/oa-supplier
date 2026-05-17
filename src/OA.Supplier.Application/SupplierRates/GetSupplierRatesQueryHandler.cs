using OA.Supplier.Domain;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Application.SupplierRates;

public static class GetSupplierRatesQueryHandler
{
  public interface IQueryHandler
  {
    Task<IEnumerable<SupplierRateDto>> HandleAsync(int supplierId, CancellationToken cancellationToken = default);
  }

  public class QueryHandler(IGetSupplierRatesBySupplierQuery query) : IQueryHandler
  {
    public async Task<IEnumerable<SupplierRateDto>> HandleAsync(int supplierId, CancellationToken cancellationToken = default)
    {
      IEnumerable<SupplierRate> supplierRates = await query.QueryAsync(supplierId, cancellationToken);
      return supplierRates.Select(SupplierRateDto.MapFrom);
    }
  }
}
