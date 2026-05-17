using FluentValidation;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.Application.Suppliers;

public static class CreateSupplier
{
  public interface ICommandHandler
  {
    Task<SupplierDto> HandleAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
  }

  public class CommandHandler(ISupplierDomainService supplierDomainService, IValidator<CreateSupplierRequest> validator) : ICommandHandler
  {
    public Task<SupplierDto> HandleAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
      validator.ValidateAndThrow(request);
    
      return supplierDomainService.CreateSupplier(request, cancellationToken)
        .ContinueWith(task => SupplierDto.MapFrom(task.Result), cancellationToken);
    }
  }
}