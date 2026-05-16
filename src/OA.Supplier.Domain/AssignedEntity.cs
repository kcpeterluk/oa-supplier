namespace OA.Supplier.Domain;

public abstract class AssignedEntity
{
    public AssignedEntity()
    {
        Id = Ulid.NewUlid();
        CreatedOn = DateTime.UtcNow;
    }
    
    public Ulid Id { get; init; }

    public required string CreatedByUser { get; init; }
  
    public DateTime CreatedOn { get; init; }
}