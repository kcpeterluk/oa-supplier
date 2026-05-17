namespace OA.Supplier.Domain.SupplierRates;

public record SupplierRateProjection(
  int Id,
  int SupplierId,
  decimal Rate,
  DateOnly RateStartDate,
  DateOnly? RateEndDate,
  string CreatedByUser,
  DateTime CreatedOn);

