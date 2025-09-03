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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            // Configure the relationship with OrderItem
            builder.HasMany(o => o.OrderItems) // An Order has many OrderItems
                   .WithOne(oi => oi.Order) // An OrderItem belongs to one Order
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure the one-to-one relationship with Payment
            builder.HasOne<Payment>() // An Order has one Payment
                   .WithOne(p => p.Order) // A Payment has one Order
                   .HasForeignKey<Payment>(p => p.OrderId) // Foreign key is in Payment
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
