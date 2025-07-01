namespace MiniETicaret.Domain.Entities;

public class Favourite:BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    public string UserId { get; set; }
    public AppUser AppUser { get; set; }
}
