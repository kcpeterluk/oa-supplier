using Microsoft.EntityFrameworkCore;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Infrastructure.Persistence.Suppliers;

internal class SupplierRepository(SupplierDbContext supplierDbContext) : ISupplierRepository
{
  public Task<Domain.Suppliers.Supplier> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public async Task<IEnumerable<Domain.Suppliers.Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await supplierDbContext.Supplier.ToArrayAsync(cancellationToken);
  }

  public async Task<Domain.Suppliers.Supplier> AddAsync(Domain.Suppliers.Supplier supplier, CancellationToken cancellationToken = default)
  {
    supplierDbContext.Supplier.Add(supplier);
    await supplierDbContext.SaveChangesAsync(cancellationToken);
    return supplier;
  }

  public Task<bool> UpdateAsync(Domain.Suppliers.Supplier supplier, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}