using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OA.Supplier.Infrastructure.Persistence.Suppliers;

internal class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Suppliers.Supplier>
{
  public void Configure(EntityTypeBuilder<Domain.Suppliers.Supplier> builder)
  {
    builder.ToTable("Suppliers");
    
    builder.HasKey(x => x.Id);
    
    builder.Property(x => x.Name)
      .IsRequired()
      .HasMaxLength(450);
    
    builder.Property(x => x.Address)
      .IsRequired()
      .HasMaxLength(450);
    
    builder.Property(x => x.CreatedByUser)
      .IsRequired()
      .HasMaxLength(450);
    
    builder.Property(x => x.CreatedOn)
      .IsRequired()
      .HasDefaultValueSql("getdate()");
  }
}