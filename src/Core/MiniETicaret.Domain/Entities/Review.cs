namespace MiniETicaret.Domain.Entities;

public class Review:BaseEntity
{


    public bool IsApproved { get; set; } // Moderator təsdiqləməsi üçün
    public DateTime? ApprovedAt { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public string? Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }

}
