using FluentValidation;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public class UpdateSupplierRequestValidator : AbstractValidator<UpdateSupplierRequest>
{
  public UpdateSupplierRequestValidator()
  {
    RuleFor(x => x.Id).GreaterThan(0);
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.Address).NotEmpty();
  }
}