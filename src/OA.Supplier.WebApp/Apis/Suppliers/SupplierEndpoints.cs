using OA.Supplier.Application.Suppliers;
using OA.Supplier.WebApp.Apis;

namespace OA.Supplier.WebApp.Apis.Suppliers;

public static class SupplierEndpoints
{
  public static RouteGroupBuilder MapSupplierEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder
      .MapGroup("/api/suppliers")
      .WithTags("Suppliers");
    
    group.MapGet("", async (
        GetSuppliersWithRatesQueryHandler.IQueryHandler queryHandler,
        CancellationToken cancellationToken) =>
      {
        IEnumerable<SupplierWithRatesDto> suppliers = await queryHandler.HandleAsync(cancellationToken);
        return TypedResults.Ok(new ApiResponse<IEnumerable<SupplierWithRatesDto>>(suppliers));
      })
      .WithName("GetSuppliers");
    
    return group;
  }
}
