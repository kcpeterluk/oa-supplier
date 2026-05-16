namespace OA.Supplier.Domain;

public abstract class AggregateRoot : AssignedEntity
{
  public required string CreatedByUser { get; init; }
  
  public DateTime CreatedOn { get; } = DateTime.UtcNow;
}