using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class GetAllSuppliersTests : WebApplicationTestBase
{
  [Test]
  public async Task GetAll_ExistingSuppliers_ReturnsSuppliers()
  {
    string firstSupplierName = $"Test Supplier:{Guid.CreateVersion7()}";
    string secondSupplierName = $"Test Supplier:{Guid.CreateVersion7()}";

    CreateSupplierRequest firstSupplierRequest = new(firstSupplierName, "123 Test Street", "Test User");
    CreateSupplierRequest secondSupplierRequest = new(secondSupplierName, "456 Test Avenue", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    GetSuppliersQueryHandler.IQueryHandler getSuppliersQueryHandler = scope.ServiceProvider.GetRequiredService<GetSuppliersQueryHandler.IQueryHandler>();

    SupplierDto firstSupplier = await createSupplierCommandHandler.HandleAsync(firstSupplierRequest);
    SupplierDto secondSupplier = await createSupplierCommandHandler.HandleAsync(secondSupplierRequest);

    IEnumerable<SupplierDto> suppliers = await getSuppliersQueryHandler.HandleAsync();

    SupplierDto? savedFirstSupplier = suppliers.SingleOrDefault(supplier => supplier.Id == firstSupplier.Id);
    SupplierDto? savedSecondSupplier = suppliers.SingleOrDefault(supplier => supplier.Id == secondSupplier.Id);

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
