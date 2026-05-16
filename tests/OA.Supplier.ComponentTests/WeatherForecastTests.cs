using System.Net;

namespace OA.Supplier.ComponentTests;

public class WeatherForecastTests : WebApplicationTestBase
{
  [Test]
  public async Task Get_ReturnsSuccess_WithSampleData()
  {
    using HttpResponseMessage response = await HttpClient.GetAsync("/weatherforecast");

    await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
  }
}