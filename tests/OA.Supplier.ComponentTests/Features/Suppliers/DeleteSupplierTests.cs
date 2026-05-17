using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain;
using OA.Supplier.Domain.Suppliers;
using SupplierEntity = OA.Supplier.Domain.Suppliers.Supplier;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class DeleteSupplierTests : WebApplicationTestBase
{
  [Test]
  public async Task Delete_ValidData_ReturnsTrue()
  {
    string supplierName = $"Test Supplier:{Guid.CreateVersion7()}";
    CreateSupplierRequest createRequest = new(supplierName, "123 Test Street", "Test User");

    SupplierDto supplier;
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    DeleteSupplier.ICommandHandler deleteSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteSupplier.ICommandHandler>();

    supplier = await createSupplierCommandHandler.HandleAsync(createRequest);
    DeleteSupplier.DeleteSupplierRequest deleteRequest = new(supplier.Id);

    bool result = await deleteSupplierCommandHandler.HandleAsync(deleteRequest);

    await Assert.That(result).IsTrue();

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<Domain.Suppliers.Supplier> supplierRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<Domain.Suppliers.Supplier>>();
    SupplierEntity? deletedSupplier = await supplierRepository.GetByIdAsync(supplier.Id);

    await Assert.That(deletedSupplier).IsNull();
  }

  [Test]
  [Arguments(0, DisplayName = "Invalid Id")]
  public async Task Delete_InvalidData_ThrowsException(int id)
  {
    DeleteSupplier.DeleteSupplierRequest request = new(id);

    using IServiceScope scope = CreateServiceScope();
    DeleteSupplier.ICommandHandler deleteSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteSupplier.ICommandHandler>();

    await Assert.That(async () => await deleteSupplierCommandHandler.HandleAsync(request))
      .Throws<ValidationException>();
  }

  [Test]
  public async Task Delete_MissingSupplier_ReturnsFalse()
  {
    DeleteSupplier.DeleteSupplierRequest request = new(int.MaxValue);

    using IServiceScope scope = CreateServiceScope();
    DeleteSupplier.ICommandHandler deleteSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteSupplier.ICommandHandler>();

    bool result = await deleteSupplierCommandHandler.HandleAsync(request);

    await Assert.That(result).IsFalse();
  }
}
