using Microsoft.EntityFrameworkCore;
using OA.Supplier.Domain;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Infrastructure.Persistence.SupplierRates;

internal class SupplierRateRepository(SupplierDbContext supplierDbContext) : IRepository<SupplierRate>
{
  public async Task<SupplierRate?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => 
    await supplierDbContext.SupplierRate.FindAsync(id, cancellationToken);

  public async Task<IEnumerable<SupplierRate>> GetAllAsync(CancellationToken cancellationToken = default) => 
    await supplierDbContext.SupplierRate.AsNoTracking().ToArrayAsync(cancellationToken);

  public async Task<SupplierRate> AddAsync(SupplierRate supplierRate, CancellationToken cancellationToken = default)
  {
    supplierDbContext.SupplierRate.Add(supplierRate);
    await supplierDbContext.SaveChangesAsync(cancellationToken);
    return supplierRate;
  }

  public async Task<bool> UpdateAsync(SupplierRate supplierRate, CancellationToken cancellationToken = default)
  {
    int result = await supplierDbContext.SaveChangesAsync(cancellationToken);
    return result > 0;
  }

  public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
  {
    SupplierRate? supplierRate = await GetByIdAsync(id, cancellationToken);
    if (supplierRate is null)
    {
      return false;
    }

    supplierDbContext.SupplierRate.Remove(supplierRate);
    await supplierDbContext.SaveChangesAsync(cancellationToken);
    return true;
  }
}