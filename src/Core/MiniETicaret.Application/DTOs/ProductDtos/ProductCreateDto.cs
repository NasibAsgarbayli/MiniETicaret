using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MiniETicaret.Application.DTOs.ProductDtos;

public class ProductCreateDto
{

    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Barcode { get; set; }
    public Guid CategoryId { get; set; }
    public IFormFile Image { get; set; }

}
