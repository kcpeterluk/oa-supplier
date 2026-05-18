namespace OA.Supplier.Domain.Suppliers;

public interface ISupplierDomainService
{
  Task<Supplier> CreateSupplier(CreateSupplierRequest request, CancellationToken cancellationToken = default);
  
  Task<bool> UpdateSupplier(UpdateSupplierRequest request, CancellationToken cancellationToken = default);
  
  Task<bool> DeleteSupplier(int id, CancellationToken cancellationToken = default);
}

public class SupplierDomainService(IRepository<Supplier> supplierRepository) : ISupplierDomainService
{
  public async Task<Supplier> CreateSupplier(CreateSupplierRequest request, CancellationToken cancellationToken = default)
  {
    Supplier newSupplier = Supplier.Create(request.Name, request.Address, request.CreatedByUser);
    await supplierRepository.AddAsync(newSupplier, cancellationToken);
    return newSupplier;
  }

  public async Task<bool> UpdateSupplier(UpdateSupplierRequest request, CancellationToken cancellationToken = default)
  {
    Supplier? supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);

    if (supplier is null)
    {
      throw new InvalidOperationException("Supplier not found");
    }

    supplier.Update(request.Name, request.Address);

    return await supplierRepository.UpdateAsync(supplier, cancellationToken);
  }

  public Task<bool> DeleteSupplier(int id, CancellationToken cancellationToken = default) => 
    supplierRepository.DeleteAsync(id, cancellationToken);
}