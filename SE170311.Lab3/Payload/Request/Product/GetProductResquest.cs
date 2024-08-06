using Microsoft.AspNetCore.Mvc;
using SE170311.Lab3.Enums;
using System.ComponentModel.DataAnnotations;

namespace SE170311.Lab3.Payload.Request.Product
{
    public class GetProductResquest
    {
        [BindProperty(Name = "search-name")]
        public string? searchName { get; set; }
        [BindProperty(Name = "category-id")]
        public Guid? categoryId { get; set; }
        [BindProperty(Name = "order-fields")]
        public string? orderFields { get; set; }
        [BindProperty(Name = "min-price")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Min Price cannot be negative")]
        public decimal minPrice { get; set; } = decimal.Zero;
        [BindProperty(Name = "max-price")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Max Price cannot be negative")]
        public decimal? maxPrice { get; set; }
        [BindProperty(Name = "product-status")]
        public ProductStatus? productStatus { get; set; }
        [BindProperty(Name = "page-index")]
        public int? pageIndex { get; set; } = 1;
        [BindProperty(Name = "page-size")]
        public int? pageSize { get; set; } = 50;
    }
}
