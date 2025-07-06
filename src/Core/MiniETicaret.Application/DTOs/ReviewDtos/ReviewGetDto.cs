namespace MiniETicaret.Application.DTOs.ReviewDtos;

public class ReviewGetDto
{
    public Guid Id { get; set; }
    public string UserFullName { get; set; }
    public Guid ProductId { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
