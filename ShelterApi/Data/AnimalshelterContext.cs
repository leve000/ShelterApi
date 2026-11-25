using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

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

public class Animal
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int? Age { get; set; }
    public double? Weight { get; set; }
    public DateTime? RegTime { get; set; }
    public Guid? ShelterId { get; set; }

    [JsonIgnore]
    public virtual Shelter? Shelter { get; set; }
}

public class Shelter
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public int Capacity { get; set; }
    public DateTime? RegTime { get; set; }

    [JsonIgnore]
    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
