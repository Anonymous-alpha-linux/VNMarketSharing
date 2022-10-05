using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Helpers;

namespace AdsMarketSharing.Entities.Configurations
{
    public class InvoiceConfig : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(i => i.CashAmount).HasColumnType("decimal(15,2)");
            //builder.Property(p => p.OnlineRef).ValueGeneratedOnAdd();
            builder.HasOne(p => p.Payment).WithOne(p => p.Invoice).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(p => p.User).WithMany(p => p.Invoices).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
 