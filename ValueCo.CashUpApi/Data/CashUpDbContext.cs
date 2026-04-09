using Microsoft.EntityFrameworkCore;
using ValueCo.CashUpApi.Models;

namespace ValueCo.CashUpApi.Data;

public class CashUpDbContext : DbContext
{
    public CashUpDbContext(DbContextOptions<CashUpDbContext> options) : base(options) { }

    public DbSet<Store> Stores => Set<Store>();
    public DbSet<CashUpDay> CashUpDays => Set<CashUpDay>();
    public DbSet<CashierRow> CashierRows => Set<CashierRow>();
    public DbSet<EFTDetail> EFTDetails => Set<EFTDetail>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Store>(e => {
            e.HasKey(s => s.StoreId);
            e.HasIndex(s => s.StoreCode).IsUnique();
            e.Property(s => s.StoreCode).HasMaxLength(20).IsRequired();
            e.Property(s => s.StoreName).HasMaxLength(100).IsRequired();
        });

        mb.Entity<CashUpDay>(e => {
            e.HasKey(d => d.DayId);
            e.HasIndex(d => new { d.StoreId, d.CashUpDate }).IsUnique();
            e.Property(d => d.FNB).HasColumnType("decimal(18,2)");
            e.Property(d => d.Surrender).HasColumnType("decimal(18,2)");
            e.Property(d => d.Floats).HasColumnType("decimal(18,2)");
            e.Property(d => d.ChangeBoxes).HasColumnType("decimal(18,2)");
            e.Property(d => d.LooseChange).HasColumnType("decimal(18,2)");
            e.Property(d => d.PettyCash).HasColumnType("decimal(18,2)");
            e.HasMany(d => d.CashierRows)
             .WithOne(r => r.CashUpDay)
             .HasForeignKey(r => r.DayId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(d => d.EFTDetails)
             .WithOne(f => f.CashUpDay)
             .HasForeignKey(f => f.DayId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<CashierRow>(e => {
            e.HasKey(r => r.RowId);
            e.Property(r => r.CashierName).HasMaxLength(100).IsRequired();
            e.Property(r => r.Cash).HasColumnType("decimal(18,2)");
            e.Property(r => r.Card).HasColumnType("decimal(18,2)");
            e.Property(r => r.EFT).HasColumnType("decimal(18,2)");
            e.Property(r => r.Erase).HasColumnType("decimal(18,2)");
            e.Property(r => r.Returns).HasColumnType("decimal(18,2)");
            e.Property(r => r.Gift).HasColumnType("decimal(18,2)");
            e.Property(r => r.Coupon).HasColumnType("decimal(18,2)");
            e.Property(r => r.Loyalty).HasColumnType("decimal(18,2)");
        });

        mb.Entity<EFTDetail>(e => {
            e.HasKey(f => f.EFTId);
            e.Property(f => f.SONumber).HasMaxLength(50);
            e.Property(f => f.Amount).HasColumnType("decimal(18,2)");
        });

        mb.Entity<AppUser>(e => {
            e.HasKey(u => u.UserId);
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            e.Property(u => u.Role).HasMaxLength(20).IsRequired();
            e.Property(u => u.StoreCode).HasMaxLength(20);
        });

    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditable)entry.Entity;
            entity.UpdatedAt = DateTime.UtcNow;
            if (entry.State == EntityState.Added)
                entity.CreatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(ct);
    }
}