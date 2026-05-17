using OA.Supplier.Application.SupplierRates;

namespace OA.Supplier.Application.Suppliers;

public record SupplierWithRatesDto(
  int Id,
  string Name,
  string Address,
  string CreatedByUser,
  DateTime CreatedOn,
  IEnumerable<SupplierRateDto> SupplierRates);

