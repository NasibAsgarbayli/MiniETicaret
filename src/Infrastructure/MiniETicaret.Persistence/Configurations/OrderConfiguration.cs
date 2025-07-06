using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniETicaret.Domain.Entities;

namespace MiniETicaret.Persistence.Configurations;

public class OrderConfiguration:IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.OrderDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(x => x.OrderProducts)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId);

        builder.HasMany(o => o.AppUsers)
            .WithOne(u => u.Order)
            .HasForeignKey(u => u.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
