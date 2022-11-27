using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsMarketSharing.Entities.Configurations
{
    public class ProductAttachmentConfig : IEntityTypeConfiguration<ProductAttachment>
    {
        public void Configure(EntityTypeBuilder<ProductAttachment> builder)
        {
            builder.HasKey(p => new { p.ProductId, p.AttachmentId });

            builder.HasOne(p => p.Product)
                .WithMany(p => p.Attachments)
                .HasForeignKey(p => p.ProductId);

            builder.HasOne(p => p.Attachment)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
