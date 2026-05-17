using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.SupplierRates;

public class GetSupplierRatesTests : WebApplicationTestBase
{
  [Test]
  public async Task GetSupplierRates_ExistingSupplierRates_ReturnsOnlySupplierRates()
  {
    CreateSupplierRequest supplierRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");
    CreateSupplierRequest otherSupplierRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "456 Test Avenue", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();
    GetSupplierRatesQueryHandler.IQueryHandler getSupplierRatesQueryHandler = scope.ServiceProvider.GetRequiredService<GetSupplierRatesQueryHandler.IQueryHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(supplierRequest);
    SupplierDto otherSupplier = await createSupplierCommandHandler.HandleAsync(otherSupplierRequest);

    SupplierRateDto firstSupplierRate = await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      new DateOnly(2026, 1, 31),
      "Test User"));
    SupplierRateDto secondSupplierRate = await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplier.Id,
      125.50m,
      new DateOnly(2026, 2, 1),
      new DateOnly(2026, 2, 28),
      "Test User"));
    await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      otherSupplier.Id,
      200.00m,
      new DateOnly(2026, 3, 1),
      new DateOnly(2026, 3, 31),
      "Test User"));

    SupplierRateDto[] supplierRates = (await getSupplierRatesQueryHandler.HandleAsync(supplier.Id)).ToArray();

    await Assert.That(supplierRates.Length).IsEqualTo(2);

    SupplierRateDto? savedFirstSupplierRate = supplierRates.SingleOrDefault(supplierRate => supplierRate.Id == firstSupplierRate.Id);
    SupplierRateDto? savedSecondSupplierRate = supplierRates.SingleOrDefault(supplierRate => supplierRate.Id == secondSupplierRate.Id);

    await Assert.That(savedFirstSupplierRate).IsNotNull();
    await Assert.That(savedFirstSupplierRate!.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(savedFirstSupplierRate.Rate).IsEqualTo(100.00m);
    await Assert.That(savedFirstSupplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 1, 1));
    await Assert.That(savedFirstSupplierRate.RateEndDate).IsEqualTo(new DateOnly(2026, 1, 31));
    await Assert.That(savedFirstSupplierRate.CreatedByUser).IsEqualTo("Test User");

    await Assert.That(savedSecondSupplierRate).IsNotNull();
    await Assert.That(savedSecondSupplierRate!.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(savedSecondSupplierRate.Rate).IsEqualTo(125.50m);
    await Assert.That(savedSecondSupplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 2, 1));
    await Assert.That(savedSecondSupplierRate.RateEndDate).IsEqualTo(new DateOnly(2026, 2, 28));
    await Assert.That(savedSecondSupplierRate.CreatedByUser).IsEqualTo("Test User");
  }

  [Test]
  public async Task GetSupplierRates_SupplierWithNoRates_ReturnsEmpty()
  {
    CreateSupplierRequest supplierRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    GetSupplierRatesQueryHandler.IQueryHandler getSupplierRatesQueryHandler = scope.ServiceProvider.GetRequiredService<GetSupplierRatesQueryHandler.IQueryHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(supplierRequest);

    SupplierRateDto[] supplierRates = (await getSupplierRatesQueryHandler.HandleAsync(supplier.Id)).ToArray();

    await Assert.That(supplierRates.Length).IsEqualTo(0);
  }
}
