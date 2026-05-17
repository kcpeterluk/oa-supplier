namespace OA.Supplier.Application.SupplierRates;

public record SupplierRateDto(int Id, int SupplierId, decimal Rate, DateOnly RateStartDate, DateOnly RateEndDate, string CreatedByUser, DateTime CreatedOn)
{
  public static SupplierRateDto MapFrom(Domain.SupplierRates.SupplierRate supplierRate)
  {
    return new SupplierRateDto(
      supplierRate.Id,
      supplierRate.SupplierId,
      supplierRate.Rate,
      supplierRate.RateStartDate,
      supplierRate.RateEndDate,
      supplierRate.CreatedByUser,
      supplierRate.CreatedOn);
  }
}
