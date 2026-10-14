
using AdvancedEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedEFCore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed 3 categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" },
            new Category { Id = 3, Name = "Clothing" }
        );

        // Seed 10 products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 25000m, CategoryId = 1 },
            new Product { Id = 2, Name = "Mouse", Price = 350m, CategoryId = 1 },
            new Product { Id = 3, Name = "Keyboard", Price = 800m, CategoryId = 1 },
            new Product { Id = 4, Name = "Monitor", Price = 6000m, CategoryId = 1 },
            new Product { Id = 5, Name = "C# Programming", Price = 500m, CategoryId = 2 },
            new Product { Id = 6, Name = "Clean Code", Price = 650m, CategoryId = 2 },
            new Product { Id = 7, Name = "ASP.NET Core", Price = 700m, CategoryId = 2 },
            new Product { Id = 8, Name = "T-Shirt", Price = 300m, CategoryId = 3 },
            new Product { Id = 9, Name = "Jeans", Price = 900m, CategoryId = 3 },
            new Product { Id = 10, Name = "Jacket", Price = 1800m, CategoryId = 3 }
        );

        modelBuilder.Entity<Product>()
        .HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Product>()
        .HasIndex(p => p.Name);
    }


    private void ApplyAuditDates()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditDates();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

}
