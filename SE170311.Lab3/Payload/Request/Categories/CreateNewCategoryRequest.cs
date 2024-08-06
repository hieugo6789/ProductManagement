using System.ComponentModel.DataAnnotations;

namespace SE170311.Lab3.Payload.Request.Categories
{
    public class CreateNewCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(50, ErrorMessage = "Category name's max length is 50 characters")]
        public string Name { get; set; } = null!;
    }

}
