using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.SupplierRates;

public class DeleteSupplierRateTests : WebApplicationTestBase
{
  [Test]
  public async Task Delete_ValidData_ReturnsTrue()
  {
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();
    DeleteSupplierRate.ICommandHandler deleteSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(CreateSupplierRequest());
    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      new DateOnly(2026, 1, 31),
      "Test User"));

    DeleteSupplierRate.DeleteSupplierRateRequest request = new(supplierRate.Id);

    bool result = await deleteSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(result).IsTrue();

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<SupplierRate> supplierRateRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<SupplierRate>>();
    SupplierRate? deletedSupplierRate = await supplierRateRepository.GetByIdAsync(supplierRate.Id);

    await Assert.That(deletedSupplierRate).IsNull();
  }

  [Test]
  [Arguments(0, DisplayName = "Invalid Id")]
  public async Task Delete_InvalidData_ThrowsException(int id)
  {
    DeleteSupplierRate.DeleteSupplierRateRequest request = new(id);

    using IServiceScope scope = CreateServiceScope();
    DeleteSupplierRate.ICommandHandler deleteSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteSupplierRate.ICommandHandler>();

    await Assert.That(async () => await deleteSupplierRateCommandHandler.HandleAsync(request))
      .Throws<ValidationException>();
  }

  [Test]
  public async Task Delete_MissingSupplierRate_ReturnsFalse()
  {
    DeleteSupplierRate.DeleteSupplierRateRequest request = new(int.MaxValue);

    using IServiceScope scope = CreateServiceScope();
    DeleteSupplierRate.ICommandHandler deleteSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<DeleteSupplierRate.ICommandHandler>();

    bool result = await deleteSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(result).IsFalse();
  }

  private static CreateSupplierRequest CreateSupplierRequest() => 
    new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");
}
