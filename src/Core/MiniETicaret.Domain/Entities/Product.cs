using static System.Net.Mime.MediaTypeNames;

namespace MiniETicaret.Domain.Entities;

public class Product : BaseEntity
{
    public string ImageUrl { get; set; }
    public string Name { get; set; }
    public int Stock { get; set; }                 // Anbarda olan say
    public bool IsActive { get; set; } = true;     // Məhsulun aktiv/satışda olması
    public string? Barcode { get; set; }               // Unikal məhsul kodu (SKU)
    public double? AverageRating { get; set; }     // Orta rəy balı (hesablanır)
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public AppUser AppUser { get; set; }
    public string UserId { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

    public ICollection<Favourite> Favourites { get; set; }
    public ICollection<Image> Images { get; set; }

    public ICollection<Review> Reviews { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; }
}
