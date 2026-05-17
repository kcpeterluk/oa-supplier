namespace OA.Supplier.WebApp.Apis.Suppliers;

public static class SupplierEndpoints
{
  public static RouteGroupBuilder MapSupplierEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder
      .MapGroup("/api/suppliers")
      .WithTags("Suppliers");
    
    group.MapGet("", () =>
      {
        SupplierModel[] suppliers = new SupplierModel[]
        {
          new(1, "Supplier 1", "Address 1", 100.00m, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1))),
          new(2, "Supplier 2", "Address 2", 200.00m, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1))),
          new(3, "Supplier 3", "Address 3", 300.00m, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1))),
          new(4, "Supplier 4", "Address 4", 400.00m, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1))),
          new(5, "Supplier 5", "Address 5", 500.00m, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1)))
        };
        return suppliers;
      })
      .WithName("GetSuppliers");
    
    return group;
  }
}

record SupplierModel(
  int Id,
  string Name,
  string Address,
  Decimal Rate,
  DateOnly RateStartDate,
  DateOnly? RateEndDate
);
