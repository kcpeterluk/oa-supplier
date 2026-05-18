using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OA.Supplier.Application.SupplierRates;
using OA.Supplier.Application.Suppliers;
using OA.Supplier.Domain.SupplierRates;
using OA.Supplier.Domain.Suppliers;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class SuppliersOverlapEndpointTests : WebApplicationTestBase
{
  private static int _yearSeed = 1800;
  private static int _openEndedYearSeed = 9000;

  [Test]
  public async Task Get_OverlappingSuppliers_WhenUnauthenticated_ReturnsUnauthorized()
  {
    using HttpResponseMessage response = await HttpClient.GetAsync("/api/suppliers/overlaps");

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
  }

  [Test]
  public async Task Get_OverlappingSuppliers_ReturnsOnlySuppliersWithOverlappingRates()
  {
    int year = GetUniqueYear();

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto firstSupplier = await CreateSupplierAsync(createSupplierCommandHandler);
    SupplierDto secondSupplier = await CreateSupplierAsync(createSupplierCommandHandler);

    SupplierRateDto firstOverlappingRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      firstSupplier.Id,
      100.00m,
      new DateOnly(year, 1, 1),
      new DateOnly(year, 1, 31));
    SupplierRateDto firstNonOverlappingRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      firstSupplier.Id,
      125.00m,
      new DateOnly(year, 2, 1),
      new DateOnly(year, 2, 28));
    SupplierRateDto secondOverlappingRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      secondSupplier.Id,
      200.00m,
      new DateOnly(year, 1, 15),
      new DateOnly(year, 1, 20));
    SupplierRateDto secondNonOverlappingRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      secondSupplier.Id,
      225.00m,
      new DateOnly(year, 3, 1),
      new DateOnly(year, 3, 31));

    await AuthenticateHttpClientAsync();
    using HttpResponseMessage response = await HttpClient.GetAsync("/api/suppliers/overlaps");
    SupplierApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<SupplierApiResponse>();

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(apiResponse).IsNotNull();
    await Assert.That(apiResponse!.Data).IsNotNull();

    SupplierResponse? savedFirstSupplier = apiResponse.Data.SingleOrDefault(supplier => supplier.Id == firstSupplier.Id);
    SupplierResponse? savedSecondSupplier = apiResponse.Data.SingleOrDefault(supplier => supplier.Id == secondSupplier.Id);

    await Assert.That(savedFirstSupplier).IsNotNull();
    await Assert.That(savedFirstSupplier!.SupplierRates.Select(supplierRate => supplierRate.Id))
      .IsEquivalentTo([firstOverlappingRate.Id]);

    await Assert.That(savedSecondSupplier).IsNotNull();
    await Assert.That(savedSecondSupplier!.SupplierRates.Select(supplierRate => supplierRate.Id))
      .IsEquivalentTo([secondOverlappingRate.Id]);

    await Assert.That(apiResponse.Data.SelectMany(supplier => supplier.SupplierRates).Select(supplierRate => supplierRate.Id))
      .DoesNotContain(firstNonOverlappingRate.Id)
      .And
      .DoesNotContain(secondNonOverlappingRate.Id);
  }

  [Test]
  public async Task Get_OverlappingSuppliers_WithSupplierId_ReturnsOnlyRequestedSupplierRates()
  {
    int year = GetUniqueYear();

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto requestedSupplier = await CreateSupplierAsync(createSupplierCommandHandler);

    SupplierRateDto firstRequestedOverlappingRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      requestedSupplier.Id,
      100.00m,
      new DateOnly(year, 1, 1),
      new DateOnly(year, 1, 31));
    SupplierRateDto secondRequestedOverlappingRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      requestedSupplier.Id,
      200.00m,
      new DateOnly(year, 1, 15),
      new DateOnly(year, 1, 20));
    SupplierRateDto requestedNonOverlappingRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      requestedSupplier.Id,
      125.00m,
      new DateOnly(year, 3, 1),
      new DateOnly(year, 3, 31));

    await AuthenticateHttpClientAsync();
    using HttpResponseMessage response = await HttpClient.GetAsync($"/api/suppliers/overlaps?supplierId={requestedSupplier.Id}");
    SupplierApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<SupplierApiResponse>();

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(apiResponse).IsNotNull();
    await Assert.That(apiResponse!.Data.Select(supplier => supplier.Id)).IsEquivalentTo([requestedSupplier.Id]);
    await Assert.That(apiResponse.Data.Single().SupplierRates.Select(supplierRate => supplierRate.Id))
      .IsEquivalentTo([firstRequestedOverlappingRate.Id, secondRequestedOverlappingRate.Id]);
    await Assert.That(apiResponse.Data.Single().SupplierRates.Select(supplierRate => supplierRate.Id))
      .DoesNotContain(requestedNonOverlappingRate.Id);
  }

  [Test]
  public async Task Get_OverlappingSuppliers_WithNoOverlaps_ReturnsEmpty()
  {
    int year = GetUniqueYear();

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await CreateSupplierAsync(createSupplierCommandHandler);
    await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      supplier.Id,
      100.00m,
      new DateOnly(year, 1, 1),
      new DateOnly(year, 1, 31));
    await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      supplier.Id,
      125.00m,
      new DateOnly(year, 2, 1),
      new DateOnly(year, 2, 28));

    await AuthenticateHttpClientAsync();
    using HttpResponseMessage response = await HttpClient.GetAsync($"/api/suppliers/overlaps?supplierId={supplier.Id}");
    SupplierApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<SupplierApiResponse>();

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(apiResponse).IsNotNull();
    await Assert.That(apiResponse!.Data.Length).IsEqualTo(0);
  }

  [Test]
  public async Task Get_OverlappingSuppliers_NullEndDateOverlapsLaterRange()
  {
    int year = GetUniqueOpenEndedYear();

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto requestedSupplier = await CreateSupplierAsync(createSupplierCommandHandler);

    SupplierRateDto openEndedRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      requestedSupplier.Id,
      100.00m,
      new DateOnly(year, 1, 1),
      null);
    SupplierRateDto laterRange = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      requestedSupplier.Id,
      200.00m,
      new DateOnly(year, 3, 1),
      new DateOnly(year, 3, 31));

    await AuthenticateHttpClientAsync();
    using HttpResponseMessage response = await HttpClient.GetAsync($"/api/suppliers/overlaps?supplierId={requestedSupplier.Id}");
    SupplierApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<SupplierApiResponse>();

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(apiResponse).IsNotNull();
    await Assert.That(apiResponse!.Data.Select(supplier => supplier.Id)).IsEquivalentTo([requestedSupplier.Id]);
    await Assert.That(apiResponse.Data.Single().SupplierRates.Select(supplierRate => supplierRate.Id))
      .IsEquivalentTo([openEndedRate.Id, laterRange.Id]);
  }

  [Test]
  public async Task Get_OverlappingSuppliers_OpenEndedRatesOverlap()
  {
    int year = GetUniqueOpenEndedYear();

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await CreateSupplierAsync(createSupplierCommandHandler);
    SupplierRateDto firstOpenEndedRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      supplier.Id,
      100.00m,
      new DateOnly(year, 1, 1),
      null);
    SupplierRateDto secondOpenEndedRate = await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      supplier.Id,
      125.00m,
      new DateOnly(year, 2, 1),
      null);

    await AuthenticateHttpClientAsync();
    using HttpResponseMessage response = await HttpClient.GetAsync($"/api/suppliers/overlaps?supplierId={supplier.Id}");
    SupplierApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<SupplierApiResponse>();

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(apiResponse).IsNotNull();
    await Assert.That(apiResponse!.Data.Select(supplier => supplier.Id)).IsEquivalentTo([supplier.Id]);
    await Assert.That(apiResponse.Data.Single().SupplierRates.Select(supplierRate => supplierRate.Id))
      .IsEquivalentTo([firstOpenEndedRate.Id, secondOpenEndedRate.Id]);
  }

  [Test]
  public async Task Get_OverlappingSuppliers_AdjacentRangesDoNotOverlap()
  {
    int year = GetUniqueYear();

    using IServiceScope scope = CreateServiceScope();
    CreateSupplier.ICommandHandler createSupplierCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplier.ICommandHandler>();
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler = scope.ServiceProvider.GetRequiredService<CreateSupplierRate.ICommandHandler>();

    SupplierDto supplier = await CreateSupplierAsync(createSupplierCommandHandler);
    await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      supplier.Id,
      100.00m,
      new DateOnly(year, 1, 1),
      new DateOnly(year, 1, 31));
    await CreateSupplierRateAsync(
      createSupplierRateCommandHandler,
      supplier.Id,
      125.00m,
      new DateOnly(year, 2, 1),
      new DateOnly(year, 2, 28));

    await AuthenticateHttpClientAsync();
    using HttpResponseMessage response = await HttpClient.GetAsync($"/api/suppliers/overlaps?supplierId={supplier.Id}");
    SupplierApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<SupplierApiResponse>();

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(apiResponse).IsNotNull();
    await Assert.That(apiResponse!.Data.Length).IsEqualTo(0);
  }

  private static int GetUniqueYear() => Interlocked.Increment(ref _yearSeed);

  private static int GetUniqueOpenEndedYear() => Interlocked.Increment(ref _openEndedYearSeed);

  private static async Task<SupplierDto> CreateSupplierAsync(CreateSupplier.ICommandHandler createSupplierCommandHandler)
  {
    return await createSupplierCommandHandler.HandleAsync(new CreateSupplierRequest(
      $"Test Supplier:{Guid.CreateVersion7()}",
      "123 Test Street",
      "Test User"));
  }

  private static async Task<SupplierRateDto> CreateSupplierRateAsync(
    CreateSupplierRate.ICommandHandler createSupplierRateCommandHandler,
    int supplierId,
    decimal rate,
    DateOnly rateStartDate,
    DateOnly? rateEndDate)
  {
    return await createSupplierRateCommandHandler.HandleAsync(new CreateSupplierRateRequest(
      supplierId,
      rate,
      rateStartDate,
      rateEndDate,
      "Test User"));
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
