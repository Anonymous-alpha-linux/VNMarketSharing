using AdsMarketSharing.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdsMarketSharing.Entities.Configurations
{
    public class NotifyConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(p => p.Type).HasConversion(new EnumToStringConverter<NotifyType>());
            builder.Property(p => p.ShortMessage).HasMaxLength(40);
        }
    }
}
