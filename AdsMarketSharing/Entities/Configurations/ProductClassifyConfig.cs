using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class ProductClassifyConfig : IEntityTypeConfiguration<ProductClassify>
    {
        public void Configure(EntityTypeBuilder<ProductClassify> builder)
        {
            builder.HasMany(pc => pc.ProductClassifyTypes).WithOne(pt => pt.ProductClassify).HasForeignKey(pt => pt.ProductClassifyId);
        }
    }
}
