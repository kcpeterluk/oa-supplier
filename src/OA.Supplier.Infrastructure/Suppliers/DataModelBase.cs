using System.Text.Json.Serialization;
using OA.Supplier.Domain;

namespace OA.Supplier.Infrastructure.Suppliers;

public abstract class DataModelBase : AssignedEntity, IDynamoDbTable
{
  [JsonPropertyName("pk")]
  public string PK => Id.ToString();
  
  [JsonPropertyName("sk")]
  public string SK => Id.ToString();
}