using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain;
using OA.Supplier.Domain.Suppliers;
using SupplierEntity = OA.Supplier.Domain.Suppliers.Supplier;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class UpdateSupplierTests : WebApplicationTestBase
{
  [Test]
  public async Task Update_ValidData_ReturnsTrue()
  {
    string supplierName = $"Test Supplier:{Guid.CreateVersion7()}";
    CreateSupplierRequest createRequest = new(supplierName, "123 Test Street", "Test User");

    SupplierDto supplier;
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    UpdateSupplier.ICommandHandler updateSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplier.ICommandHandler>();

    supplier = await createSupplierCommandHandler.HandleAsync(createRequest);
    UpdateSupplierRequest updateRequest = new(supplier.Id, "Updated Test Supplier", "456 Updated Street");

    bool result = await updateSupplierCommandHandler.HandleAsync(updateRequest);

    await Assert.That(result).IsTrue();

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<Domain.Suppliers.Supplier> supplierRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<Domain.Suppliers.Supplier>>();
    SupplierEntity? updatedSupplier = await supplierRepository.GetByIdAsync(supplier.Id);

    await Assert.That(updatedSupplier).IsNotNull();
    await Assert.That(updatedSupplier!.Name).IsEqualTo("Updated Test Supplier");
    await Assert.That(updatedSupplier.Address).IsEqualTo("456 Updated Street");
  }

  [Test]
  [Arguments(0, "Test Supplier", "123 Test Street", DisplayName = "Invalid Id")]
  [Arguments(1, "", "123 Test Street", DisplayName = "Missing Name")]
  [Arguments(1, "Test Supplier", "", DisplayName = "Missing Address")]
  public async Task Update_InvalidData_ThrowsException(int id, string name, string address)
  {
    UpdateSupplierRequest request = new(id, name, address);

    using IServiceScope scope = CreateServiceScope();
    UpdateSupplier.ICommandHandler updateSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplier.ICommandHandler>();

    await Assert.That(async () => await updateSupplierCommandHandler.HandleAsync(request))
      .Throws<ValidationException>();
  }

  [Test]
  public async Task Update_MissingSupplier_ThrowsException()
  {
    UpdateSupplierRequest request = new(int.MaxValue, "Updated Test Supplier", "456 Updated Street");

    using IServiceScope scope = CreateServiceScope();
    UpdateSupplier.ICommandHandler updateSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplier.ICommandHandler>();

    await Assert.That(async () => await updateSupplierCommandHandler.HandleAsync(request))
      .Throws<InvalidOperationException>();
  }
}
