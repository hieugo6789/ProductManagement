using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SE170311.Lab3.Repo.Models;

public partial class Category
{
    public Category()
    {
        Products = new HashSet<Product>();
    }
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
