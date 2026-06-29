using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Model;

namespace SistemaBiblioteca.Services;

public class ReglasNegocioService
{
    public const int EstadoPrestamoActivo = 1;
    public const int EstadoPrestamoDevuelto = 2;
    public const int EstadoPrestamoVencido = 3;
    public const int EstadoReservaPendiente = 4;
    public const int EstadoReservaCumplida = 5;
    public const int EstadoReservaCancelada = 6;

    private readonly SistemaBibliotecaContext _context;

    public ReglasNegocioService(SistemaBibliotecaContext context)
    {
        _context = context;
    }

    public string? ValidarSocioParaPrestamo(Socio socio)
    {
        if (socio.Activo == 0)
            return "RN-01: El socio está inactivo y no puede realizar préstamos.";

        if (TieneMultasPendientes(socio.NroSocio))
            return "RN-02: El socio tiene multas pendientes. Debe abonarlas antes de retirar libros.";

        if (!PuedeTomarMasLibros(socio))
        {
            var tipo = socio.IdTipoSocioNavigation;
            return $"RN-04: El socio alcanzó el límite de {tipo.MaxLibrosSimultaneos} libros simultáneos.";
        }

        return null;
    }

    public bool TieneMultasPendientes(int nroSocio)
    {
        return _context.Multas.Any(m => m.NroSocio == nroSocio && m.Abonada == 0);
    }

    public int ObtenerCopiasDisponibles(string isbn)
    {
        var libro = _context.Libros.Single(l => l.Isbn == isbn);
        var prestados = _context.Prestamos.Count(p => p.Isbn == isbn && p.FechaDevolucion == null);
        return libro.CantidadCopias - prestados;
    }

    public bool HayCopiasDisponibles(string isbn)
    {
        return ObtenerCopiasDisponibles(isbn) > 0;
    }

    public int ContarPrestamosActivos(int nroSocio)
    {
        return _context.Prestamos.Count(p => p.NroSocio == nroSocio && p.FechaDevolucion == null);
    }

    public bool PuedeTomarMasLibros(Socio socio)
    {
        var prestamosActivos = ContarPrestamosActivos(socio.NroSocio);
        return prestamosActivos < socio.IdTipoSocioNavigation.MaxLibrosSimultaneos;
    }

    public DateOnly CalcularFechaVencimiento(DateOnly fechaPrestamo, TipoSocio tipoSocio)
    {
        return fechaPrestamo.AddDays(tipoSocio.DiasPrestamo);
    }

    public Socio? ObtenerSocioConTipo(int nroSocio)
    {
        return _context.Socios
            .Include(s => s.IdTipoSocioNavigation)
            .SingleOrDefault(s => s.NroSocio == nroSocio);
    }
}
