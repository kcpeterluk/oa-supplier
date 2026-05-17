using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class SuppliersEndpointTests : WebApplicationTestBase
{
  [Test]
  public async Task Get_AllSuppliers_ReturnsSuppliersWithRates()
  {
    DateTime expectedCreatedOn = DateTime.UtcNow;
    CreateSupplierRequest supplierRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "123 Test Street", "Test User");
    CreateSupplierRequest supplierWithNoRatesRequest = new($"Test Supplier:{Guid.CreateVersion7()}", "456 Test Avenue", "Test User");

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await createSupplierCommandHandler.HandleAsync(supplierRequest);
    SupplierDto supplierWithNoRates = await createSupplierCommandHandler.HandleAsync(supplierWithNoRatesRequest);
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
      null,
      "Test User"));

    using HttpResponseMessage response = await HttpClient.GetAsync("/api/suppliers");
    SupplierApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<SupplierApiResponse>();

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(apiResponse).IsNotNull();
    await Assert.That(apiResponse!.Data).IsNotNull();

    SupplierResponse? savedSupplier = apiResponse.Data.SingleOrDefault(savedSupplier => savedSupplier.Id == supplier.Id);
    SupplierResponse? savedSupplierWithNoRates = apiResponse.Data.SingleOrDefault(savedSupplier => savedSupplier.Id == supplierWithNoRates.Id);

    await Assert.That(savedSupplier).IsNotNull();
    await Assert.That(savedSupplier!.Name).IsEqualTo(supplierRequest.Name);
    await Assert.That(savedSupplier.Address).IsEqualTo(supplierRequest.Address);
    await Assert.That(savedSupplier.CreatedByUser).IsEqualTo(supplierRequest.CreatedByUser);
    await Assert.That(savedSupplier.CreatedOn).IsGreaterThanOrEqualTo(expectedCreatedOn);
    await Assert.That(savedSupplier.SupplierRates.Length).IsEqualTo(2);

    SupplierRateResponse? savedFirstSupplierRate = savedSupplier.SupplierRates.SingleOrDefault(supplierRate => supplierRate.Id == firstSupplierRate.Id);
    SupplierRateResponse? savedSecondSupplierRate = savedSupplier.SupplierRates.SingleOrDefault(supplierRate => supplierRate.Id == secondSupplierRate.Id);

    await Assert.That(savedFirstSupplierRate).IsNotNull();
    await Assert.That(savedFirstSupplierRate!.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(savedFirstSupplierRate.Rate).IsEqualTo(100.00m);
    await Assert.That(savedFirstSupplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 1, 1));
    await Assert.That(savedFirstSupplierRate.RateEndDate).IsEqualTo(new DateOnly(2026, 1, 31));
    await Assert.That(savedFirstSupplierRate.CreatedByUser).IsEqualTo("Test User");
    await Assert.That(savedFirstSupplierRate.CreatedOn).IsGreaterThanOrEqualTo(expectedCreatedOn);

    await Assert.That(savedSecondSupplierRate).IsNotNull();
    await Assert.That(savedSecondSupplierRate!.SupplierId).IsEqualTo(supplier.Id);
    await Assert.That(savedSecondSupplierRate.Rate).IsEqualTo(125.50m);
    await Assert.That(savedSecondSupplierRate.RateStartDate).IsEqualTo(new DateOnly(2026, 2, 1));
    await Assert.That(savedSecondSupplierRate.RateEndDate).IsNull();
    await Assert.That(savedSecondSupplierRate.CreatedByUser).IsEqualTo("Test User");
    await Assert.That(savedSecondSupplierRate.CreatedOn).IsGreaterThanOrEqualTo(expectedCreatedOn);

    await Assert.That(savedSupplierWithNoRates).IsNotNull();
    await Assert.That(savedSupplierWithNoRates!.Name).IsEqualTo(supplierWithNoRatesRequest.Name);
    await Assert.That(savedSupplierWithNoRates.Address).IsEqualTo(supplierWithNoRatesRequest.Address);
    await Assert.That(savedSupplierWithNoRates.CreatedByUser).IsEqualTo(supplierWithNoRatesRequest.CreatedByUser);
    await Assert.That(savedSupplierWithNoRates.SupplierRates.Length).IsEqualTo(0);
  }

  private record SupplierApiResponse(SupplierResponse[] Data);

  private record SupplierResponse(
    int Id,
    string Name,
    string Address,
    string CreatedByUser,
    DateTime CreatedOn,
    SupplierRateResponse[] SupplierRates);

  private record SupplierRateResponse(
    int Id,
    int SupplierId,
    decimal Rate,
    DateOnly RateStartDate,
    DateOnly? RateEndDate,
    string CreatedByUser,
    DateTime CreatedOn);
}
