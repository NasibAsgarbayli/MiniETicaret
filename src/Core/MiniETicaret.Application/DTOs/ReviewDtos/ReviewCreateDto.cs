namespace MiniETicaret.Application.DTOs.ReviewDtos;

public class ReviewCreateDto
{
    public Guid ProductId { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
}
