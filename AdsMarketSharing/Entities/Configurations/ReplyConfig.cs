using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class ReplyConfig : IEntityTypeConfiguration<Reply>
    {
        public void Configure(EntityTypeBuilder<Reply> builder)
        {
            builder.HasOne(p => p.UserPage).WithMany(p => p.Replies).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
