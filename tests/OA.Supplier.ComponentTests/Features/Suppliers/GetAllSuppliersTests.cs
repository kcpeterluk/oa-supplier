using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Domain.Suppliers;
using SupplierEntity = OA.Supplier.Domain.Suppliers.Supplier;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class GetAllSuppliersTests : WebApplicationTestBase
{
  [Test]
  public async Task GetAll_ExistingSuppliers_ReturnsSuppliers()
  {
    string firstSupplierName = $"Test Supplier:{Guid.CreateVersion7()}";
    string secondSupplierName = $"Test Supplier:{Guid.CreateVersion7()}";

    SupplierEntity firstSupplier = SupplierEntity.Create(firstSupplierName, "123 Test Street", "Test User");
    SupplierEntity secondSupplier = SupplierEntity.Create(secondSupplierName, "456 Test Avenue", "Test User");

    using IServiceScope scope = CreateServiceScope();
    ISupplierRepository supplierRepository = scope.ServiceProvider.GetRequiredService<ISupplierRepository>();

    firstSupplier = await supplierRepository.AddAsync(firstSupplier);
    secondSupplier = await supplierRepository.AddAsync(secondSupplier);

    IEnumerable<SupplierEntity> suppliers = await supplierRepository.GetAllAsync();

    SupplierEntity? savedFirstSupplier = suppliers.SingleOrDefault(supplier => supplier.Id == firstSupplier.Id);
    SupplierEntity? savedSecondSupplier = suppliers.SingleOrDefault(supplier => supplier.Id == secondSupplier.Id);

    await Assert.That(savedFirstSupplier).IsNotNull();
    await Assert.That(savedFirstSupplier!.Name).IsEqualTo(firstSupplierName);
    await Assert.That(savedFirstSupplier.Address).IsEqualTo("123 Test Street");
    await Assert.That(savedFirstSupplier.CreatedByUser).IsEqualTo("Test User");

    await Assert.That(savedSecondSupplier).IsNotNull();
    await Assert.That(savedSecondSupplier!.Name).IsEqualTo(secondSupplierName);
    await Assert.That(savedSecondSupplier.Address).IsEqualTo("456 Test Avenue");
    await Assert.That(savedSecondSupplier.CreatedByUser).IsEqualTo("Test User");
  }
}
