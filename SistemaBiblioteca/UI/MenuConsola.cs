using SistemaBiblioteca.Model;
using SistemaBiblioteca.Services;

namespace SistemaBiblioteca.UI;

public class MenuConsola
{
    private readonly SistemaBibliotecaContext _context;
    private readonly ReglasNegocioService _reglas;
    private readonly PrestamoService _prestamoService;

    public MenuConsola(SistemaBibliotecaContext context)
    {
        _context = context;
        _reglas = new ReglasNegocioService(context);
        _prestamoService = new PrestamoService(context, _reglas);
    }

    public void Ejecutar()
    {
        var continuar = true;

        while (continuar)
        {
            Console.WriteLine("\n=============================================");
            Console.WriteLine("       SISTEMA DE GESTIÓN DE BIBLIOTECA");
            Console.WriteLine("=============================================");
            Console.WriteLine("1. Listar libros");
            Console.WriteLine("2. Realizar préstamo");
            Console.WriteLine("3. Registrar devolución      [pendiente]");
            Console.WriteLine("4. Reservar libro            [pendiente]");
            Console.WriteLine("5. Ver detalle de socio      [pendiente]");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");

            var opcion = Console.ReadLine()?.Trim();

            switch (opcion)
            {
                case "1":
                    ListarLibros();
                    break;
                case "2":
                    _prestamoService.RealizarPrestamo();
                    break;
                case "3":
                    Console.WriteLine("\nOpción pendiente: flujo de devolución (Parte 2 - segunda mitad).");
                    break;
                case "4":
                    Console.WriteLine("\nOpción pendiente: flujo de reserva (Parte 2 - segunda mitad).");
                    break;
                case "5":
                    Console.WriteLine("\nOpción pendiente: detalle del socio (Parte 2 - segunda mitad).");
                    break;
                case "0":
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    private void ListarLibros()
    {
        Console.WriteLine("\n--- LIBROS REGISTRADOS ---");

        var libros = _context.Libros.OrderBy(l => l.Titulo).ToList();

        if (libros.Count == 0)
        {
            Console.WriteLine("No hay libros registrados en la biblioteca.");
            return;
        }

        foreach (var libro in libros)
        {
            var disponibles = _reglas.ObtenerCopiasDisponibles(libro.Isbn);
            Console.WriteLine($"[ISBN: {libro.Isbn}] {libro.Titulo}");
            Console.WriteLine($"   Autor:  {libro.Autor}");
            Console.WriteLine($"   Género: {libro.Genero}");
            Console.WriteLine($"   Copias: {disponibles} disponibles / {libro.CantidadCopias} totales");
            Console.WriteLine("---------------------------------------------");
        }
    }
}
