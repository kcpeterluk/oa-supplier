using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class CreateSupplierTests : WebApplicationTestBase
{
  [Test]
  public async Task Create_ValidData_ReturnsSupplier()
  {
    DateTime expectedCreatedOn = DateTime.UtcNow;
    string supplierName = $"Test Supplier:{Guid.CreateVersion7()}";
    CreateSupplierRequest request = new(supplierName, "123 Test Street", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(request);

    await Assert.That(supplier.Id).IsGreaterThan(0);
    await Assert.That(supplier.Name).IsEqualTo(supplierName);
    await Assert.That(supplier.Address).IsEqualTo("123 Test Street");
    await Assert.That(supplier.CreatedByUser).IsEqualTo("Test User");
    await Assert.That(supplier.CreatedOn).IsGreaterThanOrEqualTo(expectedCreatedOn);
  }
    
  [Test]
  [Arguments("", "123 Test Street", "Test User", DisplayName = "Missing Name")]
  [Arguments("Test Supplier", "", "Test User", DisplayName = "Missing Address")]
  [Arguments("Test Supplier", "123 Test Street", "", DisplayName = "Missing CreatedByUser")]
  public async Task Create_InvalidData_ThrowsException(string name, string address, string createdByUser)
  {
    CreateSupplierRequest request = new(name, address, createdByUser);
        
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
        
    await Assert.That(async () => await createSupplierCommandHandler.HandleAsync(request))
      .Throws<ValidationException>();
  }
}