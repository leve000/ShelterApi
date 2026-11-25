using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models.Entities;

namespace ShelterApi.Models;

public partial class AnimalshelterContext : DbContext
{
    public AnimalshelterContext()
    {
    }

    public AnimalshelterContext(DbContextOptions<AnimalshelterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<Shelter> Shelters { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("animals");

            entity.HasIndex(e => e.ShelterId, "ShelterId");

            entity.Property(e => e.Age)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RegTime)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.ShelterId).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Species)
                .HasMaxLength(30)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Weight).HasDefaultValueSql("'NULL'");

            entity.HasOne(d => d.Shelter).WithMany(p => p.Animals)
                .HasForeignKey(d => d.ShelterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("animals_ibfk_1");
        });

        modelBuilder.Entity<Shelter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shelters");

            entity.Property(e => e.Capacity).HasColumnType("int(11)");
            entity.Property(e => e.City).HasMaxLength(40);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RegTime)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
