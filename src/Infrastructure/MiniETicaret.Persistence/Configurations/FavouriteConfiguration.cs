using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniETicaret.Domain.Entities;

namespace MiniETicaret.Persistence.Configurations;

public class FavouriteConfiguration:IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {

        builder.HasOne(x => x.Product)
                   .WithMany(x => x.Favourites)
                   .HasForeignKey(x => x.ProductId);


        builder.HasOne(f => f.AppUser)
            .WithMany(u => u.Favourites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict); 
    }

}
