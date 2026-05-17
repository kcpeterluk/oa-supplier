using Microsoft.EntityFrameworkCore;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Infrastructure.Persistence.Suppliers;

internal class SupplierRepository(SupplierDbContext supplierDbContext) : ISupplierRepository
{
  public async Task<Domain.Suppliers.Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => 
    await supplierDbContext.Supplier.FindAsync(id, cancellationToken);

  public async Task<IEnumerable<Domain.Suppliers.Supplier>> GetAllAsync(CancellationToken cancellationToken = default) => 
    await supplierDbContext.Supplier.AsNoTracking().ToArrayAsync(cancellationToken);

  public async Task<Domain.Suppliers.Supplier> AddAsync(Domain.Suppliers.Supplier supplier, CancellationToken cancellationToken = default)
  {
    supplierDbContext.Supplier.Add(supplier);
    await supplierDbContext.SaveChangesAsync(cancellationToken);
    return supplier;
  }

  public async Task<bool> UpdateAsync(Domain.Suppliers.Supplier supplier, CancellationToken cancellationToken = default)
  {
    int result = await supplierDbContext.SaveChangesAsync(cancellationToken);
    return result > 0;
  }

  public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
  {
    Domain.Suppliers.Supplier? supplier = await GetByIdAsync(id, cancellationToken);
    if (supplier is null)
    {
      return false;
    }

    supplierDbContext.Supplier.Remove(supplier);
    await supplierDbContext.SaveChangesAsync(cancellationToken);
    return true;
  }
}