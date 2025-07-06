namespace MiniETicaret.Domain.Entities;

public class Order:BaseEntity
{
    public decimal? TotalPrice { get; set; } // Sifarişin ümumi məbləği
    public string? Note { get; set; }        // Buyer və ya admin üçün qeyd
    public string? DeliveryAddress { get; set; } // Çatdırılma ünvanı
    public string? PaymentMethod { get; set; }   // "Cash", "Card" və s.
    public DateTime OrderDate { get; set; }
    public string? Status { get; set; }

    public ICollection<OrderProduct> OrderProducts { get; set; }
    public ICollection<AppUser> AppUsers { get; set; }
}
