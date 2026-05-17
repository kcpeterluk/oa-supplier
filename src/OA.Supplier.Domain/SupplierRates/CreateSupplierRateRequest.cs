namespace OA.Supplier.Domain.SupplierRates;

public record CreateSupplierRateRequest(
  int SupplierId,
  decimal Rate,
  DateOnly RateStartDate,
  DateOnly? RateEndDate,
  string CreatedByUser);
