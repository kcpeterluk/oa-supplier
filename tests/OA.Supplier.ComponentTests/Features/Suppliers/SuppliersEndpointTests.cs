using System.Net;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class SuppliersEndpointTests : WebApplicationTestBase
{
  [Test]
  public async Task Get_AllSuppliers_ReturnsSuccess()
  {
    using HttpResponseMessage response = await HttpClient.GetAsync("/api/suppliers");

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
  }
}