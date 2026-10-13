using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Api.Entities;

namespace TradeJournal.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<Strategy> Strategies => Set<Strategy>();

    // All decimals get precision (18, 8) instead of configuring each one
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 8);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // required, creates the Identity tables

        builder.Entity<Strategy>(e =>
        {
            e.Property(s => s.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(s => new { s.UserId, s.Name }).IsUnique();

            e.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Trade>(e =>
        {
            e.Property(t => t.Symbol).HasMaxLength(20).IsRequired();

            // Store enums as text ("Long") instead of numbers (0), easier to read in the DB
            e.Property(t => t.AssetType).HasConversion<string>().HasMaxLength(20);
            e.Property(t => t.Direction).HasConversion<string>().HasMaxLength(10);

            e.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.Strategy)
                .WithMany(s => s.Trades)
                .HasForeignKey(t => t.StrategyId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(t => new { t.UserId, t.EntryDate });
            e.HasIndex(t => new { t.UserId, t.Symbol });

            e.ToTable(t => t.HasCheckConstraint("CK_Trade_Rating", "\"Rating\" BETWEEN 1 AND 5"));
        });
    }

    // Sets CreatedAt / UpdatedAt automatically
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Trade>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        foreach (var entry in ChangeTracker.Entries<Strategy>()
                     .Where(e => e.State == EntityState.Added))
        {
            entry.Entity.CreatedAt = now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}