namespace OA.Supplier.Domain.Suppliers;

public class Supplier : AggregateRoot
{ 
  public Supplier(string name, string address)
  {
    Name = name;
    Address = address;
  }
  
  public string Name { get; private set; }

  public string Address { get; private set; }
  
  public static Supplier Create(string name, string address, string createdByUser)
  {
    DomainValidationException.ThrowIfNullOrWhiteSpace(name);
    DomainValidationException.ThrowIfNullOrWhiteSpace(address);
    DomainValidationException.ThrowIfNullOrWhiteSpace(createdByUser);
    
    return new Supplier(name, address)
    {
      CreatedByUser = createdByUser
    };
  }
}