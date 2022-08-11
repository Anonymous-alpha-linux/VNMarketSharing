using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class CategoryConfig :  IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasMany(c => c.SubCategories)
                .WithOne(sc => sc.ParentCategory)
                .HasForeignKey(cs => cs.ParentCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
