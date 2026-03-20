using Microsoft.EntityFrameworkCore;
using MasterPol.Data.Models;

namespace MasterPol.Data.Database
{
    public class AppDbContext : DbContext
    {
        // Конструктор без параметров для реального использования
        public AppDbContext()
        {
        }

        // Конструктор с опциями для тестов
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerType> PartnerTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SaleHistory> SaleHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=vodoleev;Username=app;Password=123456789");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("app");

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("partners");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PartnerTypeId).HasColumnName("partner_type_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.LegalAddress).HasColumnName("legal_address");
                entity.Property(e => e.INN).HasColumnName("inn");
                entity.Property(e => e.DirectorName).HasColumnName("director_name");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.PartnerType)
                    .WithMany(e => e.Partners)
                    .HasForeignKey(e => e.PartnerTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.ToTable("partner_types");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Article).HasColumnName("article");
                entity.Property(e => e.Type).HasColumnName("type");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.MinPartnerPrice).HasColumnName("min_partner_price");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<SaleHistory>(entity =>
            {
                entity.ToTable("sale_histories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PartnerId).HasColumnName("partner_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.SaleDate).HasColumnName("sale_date");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Partner)
                    .WithMany(e => e.SaleHistories)
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(e => e.SaleHistories)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}