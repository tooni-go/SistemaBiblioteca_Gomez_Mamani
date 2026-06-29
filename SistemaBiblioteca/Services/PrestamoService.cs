using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Model;

namespace SistemaBiblioteca.Services;

public class PrestamoService
{
    private readonly SistemaBibliotecaContext _context;
    private readonly ReglasNegocioService _reglas;

    public PrestamoService(SistemaBibliotecaContext context, ReglasNegocioService reglas)
    {
        _context = context;
        _reglas = reglas;
    }

    public void RealizarPrestamo()
    {
        Console.WriteLine("\n--- REALIZAR PRÉSTAMO ---");

        if (!int.TryParse(LeerEntrada("Ingrese número de socio: "), out var nroSocio))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var socio = _reglas.ObtenerSocioConTipo(nroSocio);
        if (socio is null)
        {
            Console.WriteLine("No se encontró un socio con ese número.");
            return;
        }

        var errorValidacion = _reglas.ValidarSocioParaPrestamo(socio);
        if (errorValidacion is not null)
        {
            Console.WriteLine(errorValidacion);
            return;
        }

        var termino = LeerEntrada("Buscar libro por título o autor: ");
        if (string.IsNullOrWhiteSpace(termino))
        {
            Console.WriteLine("Debe ingresar un término de búsqueda.");
            return;
        }

        var libros = _context.Libros
            .Where(l => l.Titulo.Contains(termino) || l.Autor.Contains(termino))
            .OrderBy(l => l.Titulo)
            .ToList();

        if (libros.Count == 0)
        {
            Console.WriteLine("No se encontraron libros con ese criterio.");
            return;
        }

        Console.WriteLine("\nLibros encontrados:");
        for (var i = 0; i < libros.Count; i++)
        {
            var libro = libros[i];
            var disponibles = _reglas.ObtenerCopiasDisponibles(libro.Isbn);
            Console.WriteLine($"{i + 1}. [{libro.Isbn}] {libro.Titulo} - {libro.Autor}");
            Console.WriteLine($"   Copias disponibles: {disponibles}/{libro.CantidadCopias}");
        }

        if (!int.TryParse(LeerEntrada("\nSeleccione el número de la lista: "), out var seleccion)
            || seleccion < 1
            || seleccion > libros.Count)
        {
            Console.WriteLine("Selección inválida.");
            return;
        }

        var libroSeleccionado = libros[seleccion - 1];

        if (!_reglas.HayCopiasDisponibles(libroSeleccionado.Isbn))
        {
            Console.WriteLine("RN-03: No hay copias disponibles de ese libro.");
            Console.WriteLine("Próximamente: opción de reserva (pendiente de implementación).");
            return;
        }

        var fechaPrestamo = DateOnly.FromDateTime(DateTime.Today);
        var fechaVencimiento = _reglas.CalcularFechaVencimiento(fechaPrestamo, socio.IdTipoSocioNavigation);

        var prestamo = new Prestamo
        {
            NroSocio = socio.NroSocio,
            Isbn = libroSeleccionado.Isbn,
            FechaPrestamo = fechaPrestamo,
            FechaVencimiento = fechaVencimiento,
            FechaDevolucion = null,
            IdEstado = ReglasNegocioService.EstadoPrestamoActivo
        };

        _context.Prestamos.Add(prestamo);
        _context.SaveChanges();

        Console.WriteLine("\nPréstamo registrado correctamente.");
        Console.WriteLine($"Libro: {libroSeleccionado.Titulo}");
        Console.WriteLine($"Fecha de préstamo: {fechaPrestamo:dd/MM/yyyy}");
        Console.WriteLine($"Fecha de vencimiento: {fechaVencimiento:dd/MM/yyyy} (RN-05)");
    }

    private static string LeerEntrada(string mensaje)
    {
        Console.Write(mensaje);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}
