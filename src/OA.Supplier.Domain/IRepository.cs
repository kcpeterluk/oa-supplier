namespace OA.Supplier.Domain;

public interface IRepository<T> where T : AggregateRoot
{
  Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

  Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<T> AddAsync(T supplierRate, CancellationToken cancellationToken = default);

  Task<bool> UpdateAsync(T supplierRate, CancellationToken cancellationToken = default);

  Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}