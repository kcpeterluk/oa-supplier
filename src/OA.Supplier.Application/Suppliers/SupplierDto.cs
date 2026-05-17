namespace OA.Supplier.Application.Suppliers;

public record SupplierDto(int Id, string Name, string Address, string CreatedByUser, DateTime CreatedOn)
{
  public static SupplierDto MapFrom(Domain.Suppliers.Supplier supplier)
  {
    return new SupplierDto(supplier.Id, supplier.Name, supplier.Address, supplier.CreatedByUser, supplier.CreatedOn);
  }
}