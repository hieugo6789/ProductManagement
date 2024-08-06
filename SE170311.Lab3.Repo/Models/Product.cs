using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SE170311.Lab3.Repo.Models;

public partial class Product
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public int UnitsInStock { get; set; }

    public decimal UnitPrice { get; set; }

    public string? ImageUrl { get; set; }
    [JsonIgnore]
    public string? ImageFileName { get; set; }

    public string Status { get; set; } = null!;
    [JsonIgnore]
    public virtual Category Category { get; set; } = null!;
}
