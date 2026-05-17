using FluentValidation;

namespace OA.Supplier.Application.SupplierRates;

public class DeleteSupplierRateRequestValidator : AbstractValidator<DeleteSupplierRate.DeleteSupplierRateRequest>
{
  public DeleteSupplierRateRequestValidator()
  {
    RuleFor(x => x.Id).GreaterThan(0);
  }
}
