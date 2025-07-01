using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Configurations;

namespace MiniETicaret.Persistence.Contexts;

public class MiniETicaretDbContext:IdentityDbContext<AppUser>
{
    public MiniETicaretDbContext(DbContextOptions<MiniETicaretDbContext>options):base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Category> Categories  { get; set; }
    public DbSet<Favourite> Favourites  { get; set; }
    public DbSet<Image> Images  { get; set; }
    public DbSet<Order> Orders  { get; set; }
    public DbSet<OrderProduct> OrderProducts  { get; set; }
    public DbSet<Product> Products  { get; set; }
    public DbSet<Review> Reviews  { get; set; }
  

}
