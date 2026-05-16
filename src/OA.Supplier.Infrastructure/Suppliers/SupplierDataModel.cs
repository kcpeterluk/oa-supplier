namespace OA.Supplier.Infrastructure.Suppliers;

public class SupplierDataModel : DataModelBase
{
  public required string Name { get; init; }

  public required string Address { get; init; }
}