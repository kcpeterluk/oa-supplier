using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Infrastructure.Suppliers;

internal class SupplierRepository : ISupplierRepository
{
  public Task<Domain.Suppliers.Supplier> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<Domain.Suppliers.Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    IEnumerable<Domain.Suppliers.Supplier> ouput = new List<Domain.Suppliers.Supplier>()
    {
      new("Supplier 1", "Address 1")
      {
        CreatedByUser = "Test User"
      },
      new("Supplier 2", "Address 2")
      {
        CreatedByUser = "Test User"
      },
      new("Supplier 3", "Address 3")
      {
        CreatedByUser = "Test User"
      },
      new ("Supplier 4", "Address 4")
      {
        CreatedByUser = "Test User"
      },
      new ("Supplier 5", "Address 5")
      {
        CreatedByUser = "Test User"
      }
    };
    return Task.FromResult(ouput);
  }

  public Task<Domain.Suppliers.Supplier> AddAsync(Domain.Suppliers.Supplier supplier, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
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