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
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Configure the relationship with Order
            builder.HasMany(u => u.Orders) // An ApplicationUser has many Orders
                   .WithOne(o => o.User) // An Order has one ApplicationUser
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.NoAction); // Prevents cascading deletes

            // Configure the relationship with CartItem
            builder.HasMany(u => u.CartItems) // An ApplicationUser has many CartItems
                   .WithOne(ci => ci.User) // A CartItem has one ApplicationUser
                   .HasForeignKey(ci => ci.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
