using FluentValidation;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Application.SupplierRates;

public class UpdateSupplierRateRequestValidator : AbstractValidator<UpdateSupplierRateRequest>
{
  public UpdateSupplierRateRequestValidator()
  {
    RuleFor(x => x.Id).GreaterThan(0);
    RuleFor(x => x.SupplierId).GreaterThan(0);
    RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
    RuleFor(x => x.RateEndDate)
      .GreaterThanOrEqualTo(x => x.RateStartDate)
      .When(x => x.RateEndDate.HasValue);
  }
}
