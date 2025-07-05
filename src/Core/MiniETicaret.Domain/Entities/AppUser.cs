using Microsoft.AspNetCore.Identity;

namespace MiniETicaret.Domain.Entities;

public class AppUser:IdentityUser
{
    public string Fullname { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }


    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
    public ICollection<Product> Products { get; set; }   
    public ICollection<Order> Orders { get; set; }       
    public ICollection<Favourite> Favourites { get; set; }
    public ICollection<Review> Reviews { get; set; }

}
