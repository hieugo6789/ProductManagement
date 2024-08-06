using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace SE170311.Lab3.Repo.Models;

public partial class Lab3Context : DbContext
{
    public Lab3Context()
    {
    }

    public Lab3Context(DbContextOptions<Lab3Context> options)
        : base(options)
    {
        var dbCreater = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
        if (dbCreater != null)
        {
            if (!dbCreater.CanConnect())
            {
                dbCreater.Create();
            }

            if (!dbCreater.HasTables())
            {
                dbCreater.CreateTables();
            }
        }
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E40D15494F")
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.Password).HasMaxLength(255);

            entity.Property(e => e.Status).HasMaxLength(50);

            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.Property(e => e.ImageFileName).HasMaxLength(255);

            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.Property(e => e.ProductName).HasMaxLength(50);

            entity.Property(e => e.Status).HasMaxLength(50);

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.Status).HasMaxLength(50);
        });

        var adminRoleId = Guid.NewGuid();
        var staffRoleId = Guid.NewGuid();
        var customerRoleId = Guid.NewGuid();

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Status = "Active"
            },
            new Role
            {
                Id = staffRoleId,
                Name = "Staff",
                Status = "Active"
            },
            new Role
            {
                Id = customerRoleId,
                Name = "Customer",
                Status = "Active"
            }
        );

        var sha256 = SHA256.Create();

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "System Admin 01",
                Username = "admin1",
                Password = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes("123456"))),
                RoleId = adminRoleId,
                Status = "Active"
            },
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "System Staff 01",
                Username = "staff1",
                Password = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes("123456"))),
                RoleId = staffRoleId,
                Status = "Active"
            },
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "Customer 01",
                Username = "cus1",
                Password = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes("123456"))),
                RoleId = customerRoleId,
                Status = "Active"
            }
        );

        var catId1 = Guid.NewGuid();
        var catId2 = Guid.NewGuid();
        var catId3 = Guid.NewGuid();

        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = catId1,
                Name = "Food",
                Status = "Active"
            },
            new Category
            {
                Id = catId2,
                Name = "Drink",
                Status = "Active"
            },
            new Category
            {
                Id = catId3,
                Name = "Dessert",
                Status = "Active",
            });

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Hamburger",
                CategoryId = catId1,
                UnitsInStock = 100,
                UnitPrice = 25000,
                ImageFileName = "40305292-3267-4064-91ff-3dfbf075f648",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F40305292-3267-4064-91ff-3dfbf075f648?alt=media&token=f7b7f6bc-6d4b-4364-b0a3-f27bcb16a030",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Noodles",
                CategoryId = catId1,
                UnitsInStock = 80,
                UnitPrice = 40000,
                ImageFileName = "46e2456b-9035-42b0-9147-1ff6f3ebdaf4",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F46e2456b-9035-42b0-9147-1ff6f3ebdaf4?alt=media&token=b511188f-937a-45ec-a5fc-c3d23d6e9f22",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Sandwich",
                CategoryId = catId1,
                UnitsInStock = 90,
                UnitPrice = 30000,
                ImageFileName = "ddee069f-4bb8-43c0-b036-24675f3071e9",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2Fddee069f-4bb8-43c0-b036-24675f3071e9?alt=media&token=d830112c-e938-4046-9f2c-6d769faec41c",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Beefsteak",
                CategoryId = catId1,
                UnitsInStock = 50,
                UnitPrice = 80000,
                ImageFileName = "486cd0cc-a9c8-4233-83bb-6a052e2904bd",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F486cd0cc-a9c8-4233-83bb-6a052e2904bd?alt=media&token=f832d92c-bfb3-4149-9df4-6ee6d5febc74",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Lemonade",
                CategoryId = catId2,
                UnitsInStock = 200,
                UnitPrice = 30000,
                ImageFileName = "12808a6f-fec9-4b98-878f-e0045496c2c3",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F12808a6f-fec9-4b98-878f-e0045496c2c3?alt=media&token=b1de9eb5-b35a-4de5-97ac-3c49081de7f2",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Orange Juice",
                CategoryId = catId2,
                UnitsInStock = 200,
                UnitPrice = 38000,
                ImageFileName = "1f37b250-8861-49ab-8537-44bdbb19fa0f",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F1f37b250-8861-49ab-8537-44bdbb19fa0f?alt=media&token=40179f49-2e6e-46b7-96ba-a3e8600bcd5f",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Coffee",
                CategoryId = catId2,
                UnitsInStock = 120,
                UnitPrice = 28000,
                ImageFileName = "40b5ce5c-5c71-47c2-91c9-082582eb298f",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F40b5ce5c-5c71-47c2-91c9-082582eb298f?alt=media&token=0170c03e-28f1-42ea-a6bc-66b8ef53f770",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Pudding",
                CategoryId = catId3,
                UnitsInStock = 50,
                UnitPrice = 32000,
                ImageFileName = "b77b4c63-4f88-4efc-b398-628ec5512c3a",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2Fb77b4c63-4f88-4efc-b398-628ec5512c3a?alt=media&token=b827c529-9390-4dea-bc8f-54f2044e8041",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Ice Cream",
                CategoryId = catId3,
                UnitsInStock = 150,
                UnitPrice = 25000,
                ImageFileName = "96236fcf-5842-4274-b767-2b871cd504d2",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F96236fcf-5842-4274-b767-2b871cd504d2?alt=media&token=d48fd3b9-0808-4dbf-92e1-7749e3f7131c",
                Status = "Active"
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Cake",
                CategoryId = catId3,
                UnitsInStock = 20,
                UnitPrice = 150000,
                ImageFileName = "8d2a9045-ae31-49a9-a720-a82f89bf4a7d",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/prn231-ea891.appspot.com/o/lab01%2F8d2a9045-ae31-49a9-a720-a82f89bf4a7d?alt=media&token=c6fe50bb-5ae3-4db4-877f-eec2a01bf952",
                Status = "Active"
            }
            );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
