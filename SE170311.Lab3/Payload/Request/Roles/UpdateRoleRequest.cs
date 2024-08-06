using SE170311.Lab3.Enums;

namespace SE170311.Lab3.Payload.Request.Roles
{
    public class UpdateRoleRequest
    {
        public string Name { get; set; } = null!;
        public RoleStatus Status { get; set; }
    }
}
