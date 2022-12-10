using AdsMarketSharing.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Price).HasColumnType("decimal(9,2)").IsRequired();
            builder.Property(p => p.Status).HasDefaultValue(ProductStatus.New).HasConversion<string>();
            //builder.Property(p => p.SoldQuantity).HasComputedColumnSql("SUM(orders)");
           
        }
    }
}
