using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class GetSupplierTests : WebApplicationTestBase
{
  [Test]
  public async Task Get_ExistingSupplier_ReturnsSupplier()
  {
    string supplierName = $"Test Supplier:{Guid.CreateVersion7()}";

    CreateSupplierRequest supplierRequest = new(supplierName, "123 Test Street", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    GetSupplierQueryHandler.IQueryHandler getSupplierQueryHandler = scope.ServiceProvider.GetRequiredService<GetSupplierQueryHandler.IQueryHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(supplierRequest);

    SupplierDto? savedSupplier = await getSupplierQueryHandler.HandleAsync(supplier.Id);

    await Assert.That(savedSupplier).IsNotNull();
    await Assert.That(savedSupplier!.Id).IsEqualTo(supplier.Id);
    await Assert.That(savedSupplier.Name).IsEqualTo(supplierName);
    await Assert.That(savedSupplier.Address).IsEqualTo("123 Test Street");
    await Assert.That(savedSupplier.CreatedByUser).IsEqualTo("Test User");
  }

  [Test]
  public async Task Get_MissingSupplier_ReturnsNull()
  {
    using IServiceScope scope = CreateServiceScope();
    GetSupplierQueryHandler.IQueryHandler getSupplierQueryHandler = scope.ServiceProvider.GetRequiredService<GetSupplierQueryHandler.IQueryHandler>();

    SupplierDto? supplier = await getSupplierQueryHandler.HandleAsync(int.MaxValue);

    await Assert.That(supplier).IsNull();
  }
}
