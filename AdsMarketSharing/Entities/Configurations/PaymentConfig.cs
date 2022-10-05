using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Last4Digits).HasColumnType("char(4)").IsRequired();
            builder.HasOne(p => p.User).WithMany(p => p.Payments).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
