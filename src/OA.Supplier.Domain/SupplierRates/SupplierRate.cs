namespace OA.Supplier.Domain.SupplierRates;

public class SupplierRate : AggregateRoot
{ 
  public SupplierRate(int supplierId, decimal rate, DateOnly rateStartDate, DateOnly? rateEndDate)
  {
    SupplierId = supplierId;
    Rate = rate;
    RateStartDate = rateStartDate;
    RateEndDate = rateEndDate;
  }

  public int SupplierId { get; private set; }
  
  public decimal Rate { get; private set; }

  public DateOnly RateStartDate { get; private set; }

  public DateOnly? RateEndDate { get; private set; }
  
  public static SupplierRate Create(int supplierId, decimal rate, DateOnly rateStartDate, DateOnly? rateEndDate, string createdByUser)
  {
    if (supplierId <= 0)
    {
      throw new DomainValidationException("SupplierId must be greater than 0.");
    }
    
    ValidateRate(rate, rateStartDate, rateEndDate);
    
    return new SupplierRate(supplierId, rate, rateStartDate, rateEndDate)
    {
      CreatedByUser = createdByUser
    };
  }

  public void Update(decimal rate, DateOnly rateStartDate, DateOnly? rateEndDate)
  {
    ValidateRate(rate, rateStartDate, rateEndDate);

    Rate = rate;
    RateStartDate = rateStartDate;
    RateEndDate = rateEndDate;
  }

  private static void ValidateRate(decimal rate, DateOnly rateStartDate, DateOnly? rateEndDate)
  {
    if (rate < 0)
    {
      throw new DomainValidationException("Rate must be a positive value.");
    }
    
    if (rateEndDate.HasValue && rateStartDate > rateEndDate.Value)
    {
      throw new DomainValidationException("Rate start date must be before or equal to rate end date.");
    }
  }
}
