using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class ProductClassifyTypeConfig : IEntityTypeConfiguration<ProductClassifyType>
    {
        public void Configure(EntityTypeBuilder<ProductClassifyType> builder)
        {
            //builder.HasMany(ct => ct.ProductClassifyKey).WithOne(cd => cd.ClassifyTypeValue).HasForeignKey(ct => ct.ClassifyTypeKeyId);

            //builder.HasMany(ct => ct.ProductClassifyValue).WithOne(cd => cd.ClassifyTypeKey).HasForeignKey(ct => ct.ClassifyTypeValueId);
        }
    }
}
