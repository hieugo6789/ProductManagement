using SE170311.Lab3.Enums;

namespace SE170311.Lab3.Payload.Request.Accounts
{
    public class UpdateAccountRequest
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public AccountStatus? Status { get; set; }
    }
}
