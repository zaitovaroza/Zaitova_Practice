using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using ZaitovaLibrary.Models;

namespace ZaitovaLibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PartnerType> PartnerTypes { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesHistory> SalesHistories { get; set; }
        public DbSet<SalesPoint> SalesPoints { get; set; }

        public Partner Partner
        {
            get => default;
            set
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("app");

            // Настройка для всех DateTime свойств
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : v,
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                }
            }

            modelBuilder.Entity<Partner>()
                .HasIndex(p => p.Inn)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Article)
                .IsUnique();

            modelBuilder.Entity<SalesHistory>()
                .HasIndex(sh => sh.SaleDate);

            modelBuilder.Entity<Partner>()
                .HasOne(p => p.PartnerType)
                .WithMany(pt => pt.Partners)
                .HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesHistory>()
                .HasOne(sh => sh.Partner)
                .WithMany(p => p.SalesHistories)
                .HasForeignKey(sh => sh.PartnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesHistory>()
                .HasOne(sh => sh.Product)
                .WithMany(p => p.SalesHistories)
                .HasForeignKey(sh => sh.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}