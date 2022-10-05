﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdsMarketSharing.Entities;

namespace AdsMarketSharing.Entities.Configurations
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Total).HasColumnType("decimal(18,2)").HasComputedColumnSql("Amount * Price");
           
            builder.HasOne(p => p.Buyer).WithMany(p => p.Orders).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Merchant).WithMany(p => p.Orders).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Invoice).WithMany(p => p.Orders).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Product).WithMany(p => p.Orders).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
