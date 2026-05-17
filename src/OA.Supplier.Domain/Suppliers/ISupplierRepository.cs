namespace OA.Supplier.Domain.Suppliers;

public interface ISupplierRepository
{
  Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

  Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

  Task<bool> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);

  Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}