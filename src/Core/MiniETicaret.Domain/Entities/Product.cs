﻿using static System.Net.Mime.MediaTypeNames;

namespace MiniETicaret.Domain.Entities;

public class Product:BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; }

    public ICollection<Image> Images { get; set; }
    public ICollection<Favourite> Favourites { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; }
}
