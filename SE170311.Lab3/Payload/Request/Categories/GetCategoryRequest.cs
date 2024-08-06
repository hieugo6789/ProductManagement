using Microsoft.AspNetCore.Mvc;
using SE170311.Lab3.Enums;

namespace SE170311.Lab3.Payload.Request.Categories
{
    public class GetCategoryRequest
    {
        [BindProperty(Name = "category-status")]
        public CategoryStatus? categoryStatus { get; set; }
        [BindProperty(Name = "page-index")]
        public int? pageIndex { get; set; } = 1;
        [BindProperty(Name = "page-size")]
        public int? pageSize { get; set; } = 10;

    }
}
