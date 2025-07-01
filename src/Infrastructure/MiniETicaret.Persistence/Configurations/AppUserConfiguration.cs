using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniETicaret.Domain.Entities;

namespace MiniETicaret.Persistence.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {

        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.Fullname)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(x => x.Favourites)
                .WithOne(x => x.AppUser)
                .HasForeignKey(x => x.UserId);

            builder.HasMany(x => x.Reviews)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.HasMany(u => u.Products)
                .WithOne(p => p.AppUser)
                .HasForeignKey(p => p.UserId);

            builder.HasOne(u => u.Order)
           .WithMany(o => o.AppUsers)
           .HasForeignKey(u => u.OrderId);

        }
    }
}
