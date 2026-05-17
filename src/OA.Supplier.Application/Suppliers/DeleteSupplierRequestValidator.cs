using FluentValidation;

namespace OA.Supplier.Application.Suppliers;

public class DeleteSupplierRequestValidator : AbstractValidator<DeleteSupplier.DeleteSupplierRequest>
{
  public DeleteSupplierRequestValidator()
  {
    RuleFor(x => x.Id).GreaterThan(0);
  }
}