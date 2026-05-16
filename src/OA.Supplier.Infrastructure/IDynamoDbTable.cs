namespace OA.Supplier.Infrastructure;

public interface IDynamoDbTable
{
  public string PK { get; }
  
  public string SK { get; }
}