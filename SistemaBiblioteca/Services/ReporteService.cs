using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Model;

namespace SistemaBiblioteca.Services;

public class ReporteService : IReporteService
{
    private readonly ISistemaBibliotecaContext _context;
    private readonly IReglasNegocioService _reglas;

    public ReporteService(ISistemaBibliotecaContext context, IReglasNegocioService reglas)
    {
        _context = context;
        _reglas = reglas;
    }

    public void LibrosMasPrestados()
    {
        Console.WriteLine("\n--- LIBROS MÁS PRESTADOS (TOP 5) ---");

        var libros = _context.Libros
            .Select(l => new
            {
                l.Isbn,
                l.Titulo,
                l.Autor,
                CantidadPrestamos = l.Prestamos.Count
            })
            .OrderByDescending(l => l.CantidadPrestamos)
            .ThenBy(l => l.Titulo)
            .Take(5)
            .ToList();

        if (libros.Count == 0)
        {
            Console.WriteLine("No hay préstamos registrados.");
            return;
        }

        for (var i = 0; i < libros.Count; i++)
        {
            var libro = libros[i];
            Console.WriteLine($"{i + 1}. [{libro.Isbn}] {libro.Titulo} - {libro.Autor}");
            Console.WriteLine($"   Préstamos históricos: {libro.CantidadPrestamos}");
        }
    }

    public void SociosConMultasPendientes()
    {
        Console.WriteLine("\n--- SOCIOS CON MULTAS PENDIENTES ---");

        var socios = _context.Socios
            .Where(s => s.Multas.Any(m => m.Abonada == 0))
            .Select(s => new
            {
                s.NroSocio,
                s.Nombre,
                s.Apellido,
                MontoTotal = s.Multas.Where(m => m.Abonada == 0).Sum(m => m.Monto)
            })
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .ToList();

        if (socios.Count == 0)
        {
            Console.WriteLine("No hay socios con multas pendientes.");
            return;
        }

        foreach (var socio in socios)
        {
            Console.WriteLine($"[{socio.NroSocio}] {socio.Nombre} {socio.Apellido}");
            Console.WriteLine($"   Monto total pendiente: ${socio.MontoTotal}");
        }
    }

    public void PrestamosVencidos()
    {
        Console.WriteLine("\n--- PRÉSTAMOS VENCIDOS ---");

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        var prestamos = _context.Prestamos
            .Include(p => p.IsbnNavigation)
            .Include(p => p.NroSocioNavigation)
            .Include(p => p.IdEstadoNavigation)
            .Where(p => p.FechaDevolucion == null && p.FechaVencimiento < hoy)
            .OrderBy(p => p.FechaVencimiento)
            .ToList();

        if (prestamos.Count == 0)
        {
            Console.WriteLine("No hay préstamos vencidos sin devolver.");
            return;
        }

        foreach (var prestamo in prestamos)
        {
            var diasVencido = hoy.DayNumber - prestamo.FechaVencimiento.DayNumber;
            Console.WriteLine($"Préstamo #{prestamo.IdPrestamo} | {prestamo.IsbnNavigation.Titulo}");
            Console.WriteLine($"   Socio: {prestamo.NroSocioNavigation.Nombre} {prestamo.NroSocioNavigation.Apellido}");
            Console.WriteLine($"   Venció: {prestamo.FechaVencimiento:dd/MM/yyyy} ({diasVencido} días de demora)");
            Console.WriteLine($"   Estado: {prestamo.IdEstadoNavigation.Descripcion}");
            Console.WriteLine("---------------------------------------------");
        }
    }

    public void DisponibilidadLibro()
    {
        Console.WriteLine("\n--- DISPONIBILIDAD DE LIBRO ---");

        var criterio = LeerEntrada("Ingrese ISBN o título del libro: ");
        if (string.IsNullOrWhiteSpace(criterio))
        {
            Console.WriteLine("Debe ingresar un ISBN o título.");
            return;
        }

        var libro = _context.Libros
            .FirstOrDefault(l => l.Isbn == criterio || l.Titulo.Contains(criterio));

        if (libro is null)
        {
            Console.WriteLine("No se encontró un libro con ese ISBN o título.");
            return;
        }

        var disponibles = _reglas.ObtenerCopiasDisponibles(libro.Isbn);
        var reservasPendientes = _context.Reservas
            .Count(r => r.Isbn == libro.Isbn && r.IdEstado == ReglasNegocioService.EstadoReservaPendiente);

        Console.WriteLine($"\n[{libro.Isbn}] {libro.Titulo}");
        Console.WriteLine($"Autor: {libro.Autor}");
        Console.WriteLine($"Copias disponibles: {disponibles} / {libro.CantidadCopias}");
        Console.WriteLine($"Reservas pendientes: {reservasPendientes}");
    }

    public void HistorialSocio()
    {
        Console.WriteLine("\n--- HISTORIAL DE SOCIO ---");

        if (!int.TryParse(LeerEntrada("Ingrese número de socio: "), out var nroSocio))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var socio = _context.Socios
            .FirstOrDefault(s => s.NroSocio == nroSocio);

        if (socio is null)
        {
            Console.WriteLine("No se encontró un socio con ese número.");
            return;
        }

        var prestamos = _context.Prestamos
            .Include(p => p.IsbnNavigation)
            .Include(p => p.IdEstadoNavigation)
            .Where(p => p.NroSocio == nroSocio)
            .OrderByDescending(p => p.FechaPrestamo)
            .ToList();

        var reservas = _context.Reservas
            .Include(r => r.IsbnNavigation)
            .Include(r => r.IdEstadoNavigation)
            .Where(r => r.NroSocio == nroSocio)
            .OrderByDescending(r => r.FechaReserva)
            .ToList();

        Console.WriteLine($"\nHistorial de {socio.Nombre} {socio.Apellido} (#{socio.NroSocio})");

        Console.WriteLine($"\n--- Préstamos ({prestamos.Count}) ---");
        if (prestamos.Count == 0)
        {
            Console.WriteLine("Sin préstamos registrados.");
        }
        else
        {
            foreach (var prestamo in prestamos)
            {
                var devolucion = prestamo.FechaDevolucion?.ToString("dd/MM/yyyy") ?? "Sin devolver";
                Console.WriteLine($"- {prestamo.IsbnNavigation.Titulo}");
                Console.WriteLine($"  Préstamo: {prestamo.FechaPrestamo:dd/MM/yyyy} | Vence: {prestamo.FechaVencimiento:dd/MM/yyyy}");
                Console.WriteLine($"  Devolución: {devolucion} | Estado: {prestamo.IdEstadoNavigation.Descripcion}");
            }
        }

        Console.WriteLine($"\n--- Reservas ({reservas.Count}) ---");
        if (reservas.Count == 0)
        {
            Console.WriteLine("Sin reservas registradas.");
        }
        else
        {
            foreach (var reserva in reservas)
            {
                Console.WriteLine($"- {reserva.IsbnNavigation.Titulo}");
                Console.WriteLine($"  Fecha reserva: {reserva.FechaReserva} | Estado: {reserva.IdEstadoNavigation.Descripcion}");
            }
        }
    }

    private static string LeerEntrada(string mensaje)
    {
        Console.Write(mensaje);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}
