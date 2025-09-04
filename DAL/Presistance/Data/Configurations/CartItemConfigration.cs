using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Presistance.Data.Configurations
{
    public class CartItemConfigration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Quantity)
                   .IsRequired();

            builder.Property(ci => ci.Price)
                   .HasColumnType("decimal(18,2)");

            // Default value for CreatedAt
            builder.Property(ci => ci.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Optional UpdatedAt
            builder.Property(ci => ci.UpdatedAt)
                   .IsRequired(false);

            // Relationships
            builder.HasOne(ci => ci.User)
                   .WithMany(u => u.CartItems)
                   .HasForeignKey(ci => ci.UserId);

            builder.HasOne(ci => ci.Product)
                   .WithMany(p => p.CartItems)
                   .HasForeignKey(ci => ci.ProductId);
            builder.HasIndex(ci => new { ci.UserId, ci.ProductId })
               .IsUnique();
        }
    }
}
