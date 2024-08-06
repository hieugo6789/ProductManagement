using Microsoft.AspNetCore.Mvc;
using SE170311.Lab3.Enums;
using System.ComponentModel.DataAnnotations;

namespace SE170311.Lab3.Payload.Request.Product
{
    public class UpdateProductRequest
    {
        [FromForm(Name = "name")]
        public string ProductName { get; set; } = null!;
        [FromForm(Name = "category-id")]
        public Guid? CategoryId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Units In Stock cannot be negative")]
        [FromForm(Name = "units-in-stock")]
        public int? UnitsInStock { get; set; }
        [Range(0.0, double.MaxValue, ErrorMessage = "Unit Price cannot be negative")]
        [FromForm(Name = "unit-price")]
        public decimal? UnitPrice { get; set; }
        [FromForm(Name = "image-file")]
        public IFormFile? ImageFile { get; set; }
        [FromForm(Name = "status")]
        public ProductStatus? ProductStatus { get; set; }
    }
}
