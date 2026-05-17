using OA.Supplier.Domain;

namespace OA.Supplier.Application.Suppliers;

public static class GetSupplierQueryHandler
{
  public interface IQueryHandler
  {
    Task<SupplierDto?> HandleAsync(int id, CancellationToken cancellationToken = default);
  }

  public class QueryHandler(IRepository<Domain.Suppliers.Supplier> supplierRepository) : IQueryHandler
  {
    public async Task<SupplierDto?> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
      Domain.Suppliers.Supplier? supplier = await supplierRepository.GetByIdAsync(id, cancellationToken);
      return supplier is null ? null : SupplierDto.MapFrom(supplier);
    }
  }
}
