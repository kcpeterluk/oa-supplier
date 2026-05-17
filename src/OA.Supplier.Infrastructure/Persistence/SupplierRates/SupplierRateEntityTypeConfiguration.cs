using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OA.Supplier.Domain.SupplierRates;

namespace OA.Supplier.Infrastructure.Persistence.SupplierRates;

internal class SupplierRateEntityTypeConfiguration : IEntityTypeConfiguration<SupplierRate>
{
  public void Configure(EntityTypeBuilder<SupplierRate> builder)
  {
    builder.ToTable("SupplierRates");
    
    builder.HasKey(x => x.Id);
    
    builder.Property(x => x.Rate)
      .IsRequired()
      .HasPrecision(19, 6);
    
    builder.Property(x => x.RateStartDate)
      .IsRequired();
    
    builder.Property(x => x.RateEndDate);
    
    builder.Property(x => x.CreatedByUser)
      .IsRequired()
      .HasMaxLength(450);
    
    builder.Property(x => x.CreatedOn)
      .IsRequired()
      .HasDefaultValueSql("getdate()");
  }
}
