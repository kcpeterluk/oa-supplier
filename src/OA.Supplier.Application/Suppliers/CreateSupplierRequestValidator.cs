using FluentValidation;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
{
  public CreateSupplierRequestValidator()
  {
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.Address).NotEmpty();
  }
}