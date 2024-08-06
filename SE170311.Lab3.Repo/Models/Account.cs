using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SE170311.Lab3.Repo.Models;

public partial class Account
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Guid RoleId { get; set; }

    public string Status { get; set; } = null!;
    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;
}
