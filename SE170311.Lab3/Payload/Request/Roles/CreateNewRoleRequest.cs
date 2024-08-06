using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SE170311.Lab3.Payload.Request.Roles
{
    public class CreateNewRoleRequest
    {
        [Required(ErrorMessage = "Role name is required")]
        [MaxLength(50, ErrorMessage = "Role name's max length is 50 characters")]
        public string Name { get; set; } = null!;
    }
}
