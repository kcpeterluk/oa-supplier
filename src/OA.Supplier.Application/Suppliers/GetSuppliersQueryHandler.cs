using OA.Supplier.Domain;

namespace OA.Supplier.Application.Suppliers;

public static class GetSuppliersQueryHandler
{
  public interface IQueryHandler
  {
    Task<IEnumerable<SupplierDto>> HandleAsync(CancellationToken cancellationToken = default);
  }

  public class QueryHandler(IRepository<Domain.Suppliers.Supplier> supplierRepository) : IQueryHandler
  {
    public async Task<IEnumerable<SupplierDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
      IEnumerable<Domain.Suppliers.Supplier> suppliers = await supplierRepository.GetAllAsync(cancellationToken);
      return suppliers.Select(SupplierDto.MapFrom);
    }
  }
}
