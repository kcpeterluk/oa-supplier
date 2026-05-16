using System.Net;

namespace OA.Supplier.ComponentTests.Features.Suppliers;

public class SuppliersTests : WebApplicationTestBase
{
  [Test]
  public async Task Get_ReturnsSuccess_WithSampleData()
  {
    using HttpResponseMessage response = await HttpClient.GetAsync("/suppliers");

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
  }
}