using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(p => p.Orders).WithOne(p => p.Buyer).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Avatar).WithOne(p => p.User).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
