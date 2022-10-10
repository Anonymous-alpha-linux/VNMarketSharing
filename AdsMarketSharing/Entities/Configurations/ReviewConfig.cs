using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasOne(p => p.User).WithMany(p => p.Reviews).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Product).WithMany(p => p.Reviews).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
