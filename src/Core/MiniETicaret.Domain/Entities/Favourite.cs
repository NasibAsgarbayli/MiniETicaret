namespace MiniETicaret.Domain.Entities;

public class Favourite:BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
}
