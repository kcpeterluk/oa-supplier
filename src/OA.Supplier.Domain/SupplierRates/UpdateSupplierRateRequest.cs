namespace OA.Supplier.Domain.SupplierRates;

public record UpdateSupplierRateRequest(
  int Id,
  int SupplierId,
  decimal Rate,
  DateOnly RateStartDate,
  DateOnly? RateEndDate);
