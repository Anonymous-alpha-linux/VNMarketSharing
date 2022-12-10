using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AdsMarketSharing.Entities.Configurations
{
    public class NotifyTrackerConfig : IEntityTypeConfiguration<Notifytracker>
    {
        public void Configure(EntityTypeBuilder<Notifytracker> builder)
        {
        }
    }
}
