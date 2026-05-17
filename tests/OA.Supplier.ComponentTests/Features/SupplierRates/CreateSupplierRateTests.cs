using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.SupplierRates;

public class CreateSupplierRateTests : WebApplicationTestBase
{
  [Test]
  public async Task Create_ValidData_ReturnsSupplierRate()
  {
    DateTime expectedCreatedOn = DateTime.UtcNow;
    CreateSupplierRequest supplierRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(supplierRequest);
    CreateSupplierRateRequest request = new(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      new DateOnly(2026, 1, 31),
      "Test User");

    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(supplierRate.Id).IsGreaterThan(0);
    await Assert.That(supplierRate.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(supplierRate.Rate).IsEqualTo(100.00m);
    await Assert.That(supplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 1, 1));
    await Assert.That(supplierRate.RateEndDate).IsEqualTo(new DateOnly(2026, 1, 31));
    await Assert.That(supplierRate.CreatedByUser).IsEqualTo("Test User");
    await Assert.That(supplierRate.CreatedOn).IsGreaterThanOrEqualTo(expectedCreatedOn);

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<Domain.SupplierRates.SupplierRate> supplierRateRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<Domain.SupplierRates.SupplierRate>>();
    Domain.SupplierRates.SupplierRate? savedSupplierRate = await supplierRateRepository.GetByIdAsync(supplierRate.Id);

    await Assert.That(savedSupplierRate).IsNotNull();
    await Assert.That(savedSupplierRate!.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(savedSupplierRate.Rate).IsEqualTo(100.00m);
    await Assert.That(savedSupplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 1, 1));
    await Assert.That(savedSupplierRate.RateEndDate).IsEqualTo(new DateOnly(2026, 1, 31));
    await Assert.That(savedSupplierRate.CreatedByUser).IsEqualTo("Test User");
  }

  [Test]
  public async Task Create_NullRateEndDate_ReturnsSupplierRate()
  {
    CreateSupplierRequest supplierRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(supplierRequest);
    CreateSupplierRateRequest request = new(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      null,
      "Test User");

    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(supplierRate.Id).IsGreaterThan(0);
    await Assert.That(supplierRate.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(supplierRate.Rate).IsEqualTo(100.00m);
    await Assert.That(supplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 1, 1));
    await Assert.That(supplierRate.RateEndDate).IsNull();

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<Domain.SupplierRates.SupplierRate> supplierRateRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<Domain.SupplierRates.SupplierRate>>();
    Domain.SupplierRates.SupplierRate? savedSupplierRate = await supplierRateRepository.GetByIdAsync(supplierRate.Id);

    await Assert.That(savedSupplierRate).IsNotNull();
    await Assert.That(savedSupplierRate!.RateEndDate).IsNull();
  }

  [Test]
  public async Task Create_SameRateStartAndEndDate_ReturnsSupplierRate()
  {
    CreateSupplierRequest supplierRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(supplierRequest);
    DateOnly rateDate = new(2026, 1, 1);
    CreateSupplierRateRequest request = new(
      supplier.Id,
      100.00m,
      rateDate,
      rateDate,
      "Test User");

    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(supplierRate.Id).IsGreaterThan(0);
    await Assert.That(supplierRate.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(supplierRate.RateStartDate).IsEqualTo(rateDate);
    await Assert.That(supplierRate.RateEndDate).IsEqualTo(rateDate);
  }

  [Test]
  [Arguments(0, 100, 0, 1, "Test User", DisplayName = "Invalid SupplierId")]
  [Arguments(1, -1, 0, 1, "Test User", DisplayName = "Negative Rate")]
  [Arguments(1, 100, 1, 0, "Test User", DisplayName = "Start Date After End Date")]
  [Arguments(1, 100, 0, 1, "", DisplayName = "Missing CreatedByUser")]
  public async Task Create_InvalidData_ThrowsException(int supplierId, int rate, int startDateOffset, int endDateOffset, string createdByUser)
  {
    DateOnly baseDate = new(2026, 1, 1);
    CreateSupplierRateRequest request = new(
      supplierId,
      rate,
      baseDate.AddDays(startDateOffset),
      baseDate.AddDays(endDateOffset),
      createdByUser);

    using IServiceScope scope = CreateServiceScope();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    await Assert.That(async () => await createSupplierRateCommandHandler.HandleAsync(request))
      .Throws<ValidationException>();
  }
}
