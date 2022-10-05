using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class UserPageConfig : IEntityTypeConfiguration<UserPage>
    {
        public void Configure(EntityTypeBuilder<UserPage> builder)
        {
            builder.Property(userpage => userpage.Name).HasMaxLength(50);

            builder.HasMany(p => p.Orders).WithOne(p => p.Merchant).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
