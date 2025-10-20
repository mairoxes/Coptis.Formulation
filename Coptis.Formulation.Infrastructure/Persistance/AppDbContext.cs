using Coptis.Formulation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coptis.Formulation.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Formula> Formulas => Set<Formula>();
    public DbSet<RawMaterial> RawMaterials => Set<RawMaterial>();
    public DbSet<Substance> Substances => Set<Substance>();
    public DbSet<FormulaComponent> FormulaComponents => Set<FormulaComponent>();
    public DbSet<RawMaterialSubstance> RawMaterialSubstances => Set<RawMaterialSubstance>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Formula>(e =>
        {
            e.ToTable("Formulas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.BatchWeight).HasColumnType("decimal(18,3)");
            e.Property(x => x.WeightUnit).IsRequired().HasMaxLength(8);
            e.Property(x => x.TotalCost).HasColumnType("decimal(18,2)");
            e.Property(x => x.IsHighlighted).HasDefaultValue(false);
        });

        b.Entity<RawMaterial>(e =>
        {
            e.ToTable("RawMaterials");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.PriceAmount).HasColumnType("decimal(18,4)");
            e.Property(x => x.Currency).IsRequired().HasMaxLength(8);
            e.Property(x => x.ReferenceUnit).IsRequired().HasMaxLength(8);
        });

        b.Entity<Substance>(e =>
        {
            e.ToTable("Substances");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Name).IsUnique();
        });

        b.Entity<FormulaComponent>(e =>
        {
            e.ToTable("FormulaComponents");
            e.HasKey(x => new { x.FormulaId, x.RawMaterialId });
            e.Property(x => x.Percentage).HasColumnType("decimal(9,4)");
            e.Property(x => x.EffectiveWeight).HasColumnType("decimal(18,3)");
            e.Property(x => x.CostShare).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Formula)
             .WithMany(f => f.Components)
             .HasForeignKey(x => x.FormulaId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.RawMaterial)
             .WithMany(r => r.UsedInComponents)
             .HasForeignKey(x => x.RawMaterialId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<RawMaterialSubstance>(e =>
        {
            e.ToTable("RawMaterialSubstances");
            e.HasKey(x => new { x.RawMaterialId, x.SubstanceId });
            e.Property(x => x.Percentage).HasColumnType("decimal(9,4)");
            e.HasOne(x => x.RawMaterial)
             .WithMany(r => r.SubstanceShares)
             .HasForeignKey(x => x.RawMaterialId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Substance)
             .WithMany(s => s.InRawMaterials)
             .HasForeignKey(x => x.SubstanceId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
