using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using VoucherApp.Core.Entities;

namespace VoucherApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<RewardTemplate> RewardTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RewardTemplate>()
                .Property(p => p.Value).HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Voucher>().HasIndex(v => v.QrCodeContent).IsUnique();
            modelBuilder.Entity<Voucher>().HasIndex(v => v.ShortCode).IsUnique();
        }
    }
}