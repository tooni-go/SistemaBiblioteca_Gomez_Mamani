using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SistemaBiblioteca.Model;

public partial class SistemaBibliotecaContext : DbContext, ISistemaBibliotecaContext
{
    public SistemaBibliotecaContext()
    {
    }

    public SistemaBibliotecaContext(DbContextOptions<SistemaBibliotecaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Multa> Multas { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Socio> Socios { get; set; }

    public virtual DbSet<TipoSocio> TipoSocios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "SistemaBiblioteca.db"));
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IdEstado);

            entity.ToTable("Estado");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.Isbn);

            entity.ToTable("Libro");

            entity.Property(e => e.Isbn).HasColumnName("ISBN");
        });

        modelBuilder.Entity<Multa>(entity =>
        {
            entity.HasKey(e => e.IdMulta);

            entity.ToTable("Multa");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.Multas)
                .HasForeignKey(d => d.IdPrestamo)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.NroSocioNavigation).WithMany(p => p.Multas)
                .HasForeignKey(d => d.NroSocio)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.IdPrestamo);

            entity.ToTable("Prestamo");

            entity.Property(e => e.Isbn).HasColumnName("ISBN");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.IdEstado)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.NroSocioNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.NroSocio)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva);

            entity.ToTable("Reserva");

            entity.Property(e => e.Isbn).HasColumnName("ISBN");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdEstado)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.NroSocioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.NroSocio)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasKey(e => e.NroSocio);

            entity.ToTable("Socio");

            entity.Property(e => e.NroSocio).ValueGeneratedNever();

            entity.HasOne(d => d.IdTipoSocioNavigation).WithMany(p => p.Socios)
                .HasForeignKey(d => d.IdTipoSocio)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TipoSocio>(entity =>
        {
            entity.HasKey(e => e.IdTipoSocio);

            entity.ToTable("TipoSocio");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
