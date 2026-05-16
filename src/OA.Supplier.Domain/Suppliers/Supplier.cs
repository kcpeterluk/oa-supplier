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
}