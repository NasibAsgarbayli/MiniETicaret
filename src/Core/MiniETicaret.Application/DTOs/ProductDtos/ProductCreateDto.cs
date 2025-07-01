using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniETicaret.Application.DTOs.ProductDtos;

public class ProductCreateDto
{

    public string Title { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
}
