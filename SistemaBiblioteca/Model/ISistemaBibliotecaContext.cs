using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SistemaBiblioteca.Model;

public interface ISistemaBibliotecaContext : IDisposable
{
    DbSet<Estado> Estados { get; set; }
    DbSet<Libro> Libros { get; set; }
    DbSet<Multa> Multas { get; set; }
    DbSet<Prestamo> Prestamos { get; set; }
    DbSet<Reserva> Reservas { get; set; }
    DbSet<Socio> Socios { get; set; }
    DbSet<TipoSocio> TipoSocios { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DatabaseFacade Database { get; }
}
