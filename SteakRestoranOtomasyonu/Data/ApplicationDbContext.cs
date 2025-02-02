using Microsoft.EntityFrameworkCore;
using RestoranOtomasyonu.Models;

namespace RestoranOtomasyonu.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Örnek kategori verileri
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ana Yemekler" },
                new Category { Id = 2, Name = "İçecekler" },
                new Category { Id = 3, Name = "Tatlılar" }
            );

            // Örnek masa verileri
            modelBuilder.Entity<Table>().HasData(
                new Table { Id = 1, Number = 1, IsOccupied = false },
                new Table { Id = 2, Number = 2, IsOccupied = false },
                new Table { Id = 3, Number = 3, IsOccupied = false },
                new Table { Id = 4, Number = 4, IsOccupied = false }
            );
        }
    }
} 