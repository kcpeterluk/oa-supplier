namespace OA.Supplier.WebApi.Features.Suppliers;

public static class SupplierEndpoints
{
  public static RouteGroupBuilder MapSupplierEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    string[] summaries = new[]
    {
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    RouteGroupBuilder group = endpointRouteBuilder
      .MapGroup("/weatherforecast")
      .WithTags("Weather Forecast");
    
    group.MapGet("", () =>
      {
        WeatherForecast[] forecast =  Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
              DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
              Random.Shared.Next(-20, 55),
              summaries[Random.Shared.Next(summaries.Length)]
            ))
          .ToArray();
        return forecast;
      })
      .WithName("GetWeatherForecast");
    
    return group;
  }
}