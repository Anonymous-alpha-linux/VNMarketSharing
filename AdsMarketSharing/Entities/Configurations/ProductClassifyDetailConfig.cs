using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class ProductClassifyDetailConfig : IEntityTypeConfiguration<ProductClassfiyDetail>
    {
        public void Configure(EntityTypeBuilder<ProductClassfiyDetail> builder)
        {
            builder.Property(p => p.Price).HasColumnType("decimal(9,2)").IsRequired();

            builder.Property(p => p.Inventory).HasDefaultValue(0);

            //builder.HasKey(prop => new { prop.ClassifyTypeKeyId, prop.ClassifyTypeValueId });

            //builder.HasOne(prop => prop.ClassifyTypeKey)
            //    .WithOne()
            //    .HasForeignKey(ct => ct.Product)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(prop => prop.ClassifyTypeValue)
            //    .WithMany(ct => ct.ProductClassifyKeys)
            //    .HasForeignKey(ct => ct.ClassifyTypeValueId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(prop => prop.ClassifyTypeValue).WithMany(typeValue => typeValue.ProductClassifyKeys).HasForeignKey(ct => ct.ClassifyTypeValueId).OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(prop => prop.ClassifyTypeKey).WithMany(typeValue => typeValue.ProductClassifyValues).HasForeignKey(ct => ct.ClassifyTypeKeyId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
