namespace OA.Supplier.Domain.Suppliers;

public record CreateSupplierRequest(string Name, string Address, string CreatedByUser);