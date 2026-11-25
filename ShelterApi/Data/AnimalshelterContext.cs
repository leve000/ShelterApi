using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;

namespace ShelterApi.Data;

public class AnimalshelterContext : DbContext
{
    public AnimalshelterContext(DbContextOptions<AnimalshelterContext> options)
        : base(options)
    {
    }

    public DbSet<Animal> Animals { get; set; } = null!;
    public DbSet<Shelter> Shelters { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("animals");
            entity.HasIndex(e => e.ShelterId, "ShelterId");
            entity.Property(e => e.Age).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(40);
            entity.Property(e => e.RegTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Species).HasMaxLength(30);
            entity.Property(e => e.Weight);
            entity.HasOne(e => e.Shelter)
                .WithMany(e => e.Animals)
                .HasForeignKey(e => e.ShelterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("animals_ibfk_1");
        });

        modelBuilder.Entity<Shelter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("shelters");
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.City).HasMaxLength(40).IsRequired();
            entity.Property(e => e.Capacity).HasColumnType("int(11)");
            entity.Property(e => e.RegTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });
    }
}
