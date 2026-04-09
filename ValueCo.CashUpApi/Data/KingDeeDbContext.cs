using Microsoft.EntityFrameworkCore;
using ValueCo.CashUpApi.Models;

namespace ValueCo.CashUpApi.Data;

public class KingDeeDbContext : DbContext
{
    public KingDeeDbContext(DbContextOptions<KingDeeDbContext> options) : base(options) { }

    public DbSet<KdContribution> Contributions => Set<KdContribution>();
    public DbSet<KdJournal> Journal => Set<KdJournal>();
    public DbSet<KdSales> Sales => Set<KdSales>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<KdContribution>().HasNoKey().ToView(null);
        mb.Entity<KdJournal>().HasNoKey().ToView(null);
        mb.Entity<KdSales>().HasNoKey().ToView(null);
    }
}