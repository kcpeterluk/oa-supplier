using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace OA.Supplier.ComponentTests;

public abstract class WebApplicationTestBase
{
  [ClassDataSource<WebApplicationFactory>(Shared = SharedType.PerTestSession)]
  public WebApplicationFactory Factory { get; init; } = null!;

  public HttpClient HttpClient { get; private set; } = default!;
  
  public IServiceScope CreateServiceScope() => Factory.Services.CreateScope();
  
  [Before(Test)]
  public virtual Task Setup()
  {
    HttpClient = Factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false
    });
    
    return Task.CompletedTask;
  }
}