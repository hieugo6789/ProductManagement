using SE170311.Lab3.Enums;

namespace SE170311.Lab3.Payload.Request.Categories
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = null!;
        public CategoryStatus? Status { get; set; } = null!;
    }
}
