using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.SupplierRates;

public class UpdateSupplierRateTests : WebApplicationTestBase
{
  [Test]
  public async Task Update_ValidData_ReturnsTrue()
  {
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();
    UpdateSupplierRate.ICommandHandler updateSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(CreateSupplierRequest());
    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      new DateOnly(2026, 1, 31),
      "Test User"));

    UpdateSupplierRateRequest request = new(
      supplierRate.Id,
      supplier.Id,
      125.50m,
      new DateOnly(2026, 2, 1),
      new DateOnly(2026, 2, 28));

    bool result = await updateSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(result).IsTrue();

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<SupplierRate> supplierRateRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<SupplierRate>>();
    SupplierRate? updatedSupplierRate = await supplierRateRepository.GetByIdAsync(supplierRate.Id);

    await Assert.That(updatedSupplierRate).IsNotNull();
    await Assert.That(updatedSupplierRate!.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(updatedSupplierRate.Rate).IsEqualTo(125.50m);
    await Assert.That(updatedSupplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 2, 1));
    await Assert.That(updatedSupplierRate.RateEndDate).IsEqualTo(new DateOnly(2026, 2, 28));
    await Assert.That(updatedSupplierRate.CreatedByUser).IsEqualTo("Test User");
  }

  [Test]
  public async Task Update_NullRateEndDate_ReturnsTrue()
  {
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();
    UpdateSupplierRate.ICommandHandler updateSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(CreateSupplierRequest());
    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      new DateOnly(2026, 1, 31),
      "Test User"));

    UpdateSupplierRateRequest request = new(
      supplierRate.Id,
      supplier.Id,
      150.00m,
      new DateOnly(2026, 3, 1),
      null);

    bool result = await updateSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(result).IsTrue();

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<SupplierRate> supplierRateRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<SupplierRate>>();
    SupplierRate? updatedSupplierRate = await supplierRateRepository.GetByIdAsync(supplierRate.Id);

    await Assert.That(updatedSupplierRate).IsNotNull();
    await Assert.That(updatedSupplierRate!.Rate).IsEqualTo(150.00m);
    await Assert.That(updatedSupplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 3, 1));
    await Assert.That(updatedSupplierRate.RateEndDate).IsNull();
  }

  [Test]
  public async Task Update_SameRateStartAndEndDate_ReturnsTrue()
  {
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();
    UpdateSupplierRate.ICommandHandler updateSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(CreateSupplierRequest());
    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      null,
      "Test User"));

    DateOnly rateDate = new(2026, 4, 1);
    UpdateSupplierRateRequest request = new(
      supplierRate.Id,
      supplier.Id,
      175.00m,
      rateDate,
      rateDate);

    bool result = await updateSupplierRateCommandHandler.HandleAsync(request);

    await Assert.That(result).IsTrue();

    using IServiceScope verificationScope = CreateServiceScope();
    IRepository<SupplierRate> supplierRateRepository = verificationScope.ServiceProvider.GetRequiredService<IRepository<SupplierRate>>();
    SupplierRate? updatedSupplierRate = await supplierRateRepository.GetByIdAsync(supplierRate.Id);

    await Assert.That(updatedSupplierRate).IsNotNull();
    await Assert.That(updatedSupplierRate!.RateStartDate).IsEqualTo(rateDate);
    await Assert.That(updatedSupplierRate.RateEndDate).IsEqualTo(rateDate);
  }

  [Test]
  [Arguments(0, 1, 100, 0, 1, DisplayName = "Invalid Id")]
  [Arguments(1, 0, 100, 0, 1, DisplayName = "Invalid SupplierId")]
  [Arguments(1, 1, -1, 0, 1, DisplayName = "Negative Rate")]
  [Arguments(1, 1, 100, 1, 0, DisplayName = "Start Date After End Date")]
  public async Task Update_InvalidData_ThrowsException(int id, int supplierId, int rate, int startDateOffset, int endDateOffset)
  {
    DateOnly baseDate = new(2026, 1, 1);
    UpdateSupplierRateRequest request = new(
      id,
      supplierId,
      rate,
      baseDate.AddDays(startDateOffset),
      baseDate.AddDays(endDateOffset));

    using IServiceScope scope = CreateServiceScope();
    UpdateSupplierRate.ICommandHandler updateSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplierRate.ICommandHandler>();

    await Assert.That(async () => await updateSupplierRateCommandHandler.HandleAsync(request))
      .Throws<ValidationException>();
  }

  [Test]
  public async Task Update_MissingSupplierRate_ThrowsException()
  {
    UpdateSupplierRateRequest request = new(
      int.MaxValue,
      1,
      100.00m,
      new DateOnly(2026, 1, 1),
      new DateOnly(2026, 1, 31));

    using IServiceScope scope = CreateServiceScope();
    UpdateSupplierRate.ICommandHandler updateSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplierRate.ICommandHandler>();

    await Assert.That(async () => await updateSupplierRateCommandHandler.HandleAsync(request))
      .Throws<InvalidOperationException>();
  }

  [Test]
  public async Task Update_DifferentSupplierId_ThrowsException()
  {
    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();
    UpdateSupplierRate.ICommandHandler updateSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<UpdateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(CreateSupplierRequest());
    SupplierDto otherSupplier = await createSupplierCommandHandler.HandleAsync(CreateSupplierRequest());
    SupplierRateDto supplierRate = await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplier.Id,
      100.00m,
      new DateOnly(2026, 1, 1),
      null,
      "Test User"));

    UpdateSupplierRateRequest request = new(
      supplierRate.Id,
      otherSupplier.Id,
      125.00m,
      new DateOnly(2026, 2, 1),
      null);

    await Assert.That(async () => await updateSupplierRateCommandHandler.HandleAsync(request))
      .Throws<InvalidOperationException>();
  }

  private static CreateSupplierRequest CreateSupplierRequest() => 
    new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");
}
