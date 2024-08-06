using System.ComponentModel.DataAnnotations;

namespace SE170311.Lab3.Payload.Request.Accounts
{
    public class CreateNewAccountRequest
    {
        [Required(ErrorMessage = "Account name is required")]
        [MaxLength(50, ErrorMessage = "Account name's max length is 50 characters")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50, ErrorMessage = "Username's max length is 50 characters")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Role Id is required")]
        public Guid RoleId { get; set; }
    }
}
