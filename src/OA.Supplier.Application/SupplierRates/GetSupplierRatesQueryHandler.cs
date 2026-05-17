using OA.Supplier.Domain;

namespace OA.Supplier.Application.SupplierRates;

public static class GetSupplierRatesQueryHandler
{
  public interface IQueryHandler
  {
    Task<IEnumerable<SupplierRateDto>> HandleAsync(int supplierId, CancellationToken cancellationToken = default);
  }

  public class QueryHandler(IRepository<Domain.SupplierRates.SupplierRate> supplierRateRepository) : IQueryHandler
  {
    public async Task<IEnumerable<SupplierRateDto>> HandleAsync(int supplierId, CancellationToken cancellationToken = default)
    {
      IEnumerable<Domain.SupplierRates.SupplierRate> supplierRates = await supplierRateRepository.GetAllAsync(cancellationToken);
      return supplierRates
        .Where(supplierRate => supplierRate.SupplierId == supplierId)
        .Select(SupplierRateDto.MapFrom);
    }
  }
}
