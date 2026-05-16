namespace OA.Supplier.Domain.Suppliers;

public interface ISupplierRepository
{
  Task<Supplier> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

  Task<bool> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);

  Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}