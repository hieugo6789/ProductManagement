using System.ComponentModel;

namespace SE170311.Lab3.Enums;

    public enum RoleEnum
    {
        [Description("Admin")]
        Admin,
        [Description("Staff")]
        Staff,
        [Description("Customer")]
        Customer
    }

