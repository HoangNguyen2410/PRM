using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AdminProductManager.Models
{
    public class Product
    {
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public string Brand { get; set; } = "ROG";
        public string? Category { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<int>? Size { get; set; }
    }
} 