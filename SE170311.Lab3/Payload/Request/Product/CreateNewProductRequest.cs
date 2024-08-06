using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SE170311.Lab3.Payload.Request.Product
{
    public class CreateNewProductRequest
    {
        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(50, ErrorMessage = "Product name's max length is 50 characters")]
        [FromForm(Name = "name")]
        public string ProductName { get; set; } = null!;
        [Required(ErrorMessage = "Category Id is required")]
        [FromForm(Name = "category-id")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Units In Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Units In Stock cannot be negative")]
        [FromForm(Name = "units-in-stock")]
        public int UnitsInStock { get; set; }
        [Required(ErrorMessage = "Unit Price is required")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Unit Price cannot be negative")]
        [FromForm(Name = "unit-price")]
        public decimal UnitPrice { get; set; }
        [FromForm(Name = "image-file")]
        public IFormFile? ImageFile { get; set; }
    }
}
