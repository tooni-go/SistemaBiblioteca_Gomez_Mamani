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
    public void RegistrarDevolucion()
    {
        Console.WriteLine("\n--- REGISTRAR DEVOLUCIÓN ---");

        Console.Write("Ingrese el Número de Socio: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int nroSocio))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var socio = _reglas.ObtenerSocioConTipo(nroSocio);

        if (socio == null)
        {
            Console.WriteLine("No se encontró ningún socio con ese número.");
            return;
        }

        var prestamosActivos = _context.Prestamos
            .Include(p => p.IsbnNavigation)
            .Where(p => p.NroSocio == socio.NroSocio && p.IdEstado == ReglasNegocioService.EstadoPrestamoActivo)
            .ToList();

        if (prestamosActivos.Count == 0)
        {
            Console.WriteLine("Este socio no tiene préstamos pendientes de devolución.");
            return;
        }

        Console.WriteLine($"\nPréstamos pendientes de {socio.Nombre} {socio.Apellido}:");
        for (int i = 0; i < prestamosActivos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {prestamosActivos[i].IsbnNavigation.Titulo} (Vencía el: {prestamosActivos[i].FechaVencimiento})");
        }

        Console.Write("\nSeleccione el número del préstamo a devolver: ");
        if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= prestamosActivos.Count)
        {
            var prestamoSeleccionado = prestamosActivos[seleccion - 1];
            var fechaHoy = DateOnly.FromDateTime(DateTime.Now);

            prestamoSeleccionado.FechaDevolucion = fechaHoy;

            prestamoSeleccionado.IdEstado = ReglasNegocioService.EstadoPrestamoDevuelto;

            if (fechaHoy > prestamoSeleccionado.FechaVencimiento)
            {
                int diasRetraso = fechaHoy.DayNumber - prestamoSeleccionado.FechaVencimiento.DayNumber;
                decimal montoTotal = diasRetraso * socio.IdTipoSocioNavigation.MultaPorDia;

                var nuevaMulta = new Multa
                {
                    NroSocio = socio.NroSocio,
                    IdPrestamo = prestamoSeleccionado.IdPrestamo,
                    Monto = montoTotal,
                    DiasDemora = diasRetraso,
                    FechaGeneracion = fechaHoy,
                    Abonada = 0
                };

                _context.Multas.Add(nuevaMulta);

                Console.WriteLine($"\n[ATENCIÓN] Devolución fuera de término ({diasRetraso} días).");
                Console.WriteLine($"Se ha generado una multa de ${montoTotal} según su categoría.");
            }

            var reservaAntigua = _context.Reservas
                .Where(r => r.Isbn == prestamoSeleccionado.Isbn && r.IdEstado == ReglasNegocioService.EstadoReservaPendiente)
                .OrderBy(r => r.IdReserva)
                .FirstOrDefault();

            if (reservaAntigua != null)
            {
                reservaAntigua.IdEstado = ReglasNegocioService.EstadoReservaCumplida;
                Console.WriteLine($"\n[NOTIFICACIÓN] Se ha cumplido una reserva pendiente para el libro {prestamoSeleccionado.IsbnNavigation.Titulo}.");
            }

            _context.SaveChanges();
            Console.WriteLine("\n¡Devolución registrada con éxito!");
        }
        else
        {
            Console.WriteLine("Selección inválida.");
        }
    }

    public void RegistrarReserva()
    {
        Console.WriteLine("\n--- RESERVAR LIBRO ---");

        Console.Write("Ingrese el Número de Socio: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int nroSocio))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var socio = _context.Socios.FirstOrDefault(s => s.NroSocio == nroSocio);
        if (socio == null)
        {
            Console.WriteLine("No se encontró ningún socio con ese número.");
            return;
        }

        if (socio.Activo == 0)
        {
            Console.WriteLine("\n[ERROR] RN-01: El socio está inactivo y no puede realizar reservas.");
            return;
        }

        Console.Write("Ingrese el título, autor o ISBN del libro que desea buscar: ");
        var busqueda = Console.ReadLine()?.Trim().ToLower() ?? "";

        var librosBuscados = _context.Libros
            .Where(l => l.Titulo.ToLower().Contains(busqueda)
                     || l.Autor.ToLower().Contains(busqueda)
                     || l.Isbn.ToLower().Contains(busqueda))
            .ToList();

        if (librosBuscados.Count == 0)
        {
            Console.WriteLine("No se encontraron libros con esos datos.");
            return;
        }

        Console.WriteLine("\nLibros encontrados:");
        for (int i = 0; i < librosBuscados.Count; i++)
        {
            var disponibles = _reglas.ObtenerCopiasDisponibles(librosBuscados[i].Isbn);
            Console.WriteLine($"{i + 1}. {librosBuscados[i].Titulo} - {librosBuscados[i].Autor} (Disponibles: {disponibles})");
        }

        Console.Write("\nSeleccione el número del libro a reservar: ");
        if (!int.TryParse(Console.ReadLine(), out int seleccion) || seleccion <= 0 || seleccion > librosBuscados.Count)
        {
            Console.WriteLine("Selección inválida.");
            return;
        }

        var libroSeleccionado = librosBuscados[seleccion - 1];

        if (_reglas.HayCopiasDisponibles(libroSeleccionado.Isbn))
        {
            Console.WriteLine($"\n[AVISO] Actualmente hay copias disponibles de '{libroSeleccionado.Titulo}'. Podés realizar un préstamo normal desde la Opción 2.");
        }

        bool yaTieneReserva = _context.Reservas.Any(r =>
            r.NroSocio == socio.NroSocio &&
            r.Isbn == libroSeleccionado.Isbn &&
            r.IdEstado == ReglasNegocioService.EstadoReservaPendiente);

        if (yaTieneReserva)
        {
            Console.WriteLine($"\n[ERROR] RN-08: El socio ya tiene una reserva pendiente para el libro '{libroSeleccionado.Titulo}'.");
            return;
        }

        var nuevaReserva = new Reserva
        {
            NroSocio = socio.NroSocio,
            Isbn = libroSeleccionado.Isbn,
            FechaReserva = DateTime.Now.ToString("yyyy-MM-dd"),
            IdEstado = ReglasNegocioService.EstadoReservaPendiente
        };

        _context.Reservas.Add(nuevaReserva);
        _context.SaveChanges();

        Console.WriteLine($"\n¡Reserva registrada con éxito para el libro '{libroSeleccionado.Titulo}'!");
    }

    public void VerDetalleSocio()
    {
        Console.WriteLine("\n--- DETALLE DE SOCIO ---");

        Console.Write("Ingrese el Número de Socio: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int nroSocio))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var socio = _context.Socios
            .Include(s => s.IdTipoSocioNavigation)
            .Include(s => s.Prestamos) 
                .ThenInclude(p => p.IsbnNavigation)
            .Include(s => s.Multas) 
            .FirstOrDefault(s => s.NroSocio == nroSocio);

        if (socio == null)
        {
            Console.WriteLine("No se encontró ningún socio con ese número.");
            return;
        }

        Console.WriteLine($"\n=============================================");
        Console.WriteLine($" FICHA DEL SOCIO: {socio.Nombre} {socio.Apellido}");
        Console.WriteLine($"=============================================");
        Console.WriteLine($"Nro Socio: {socio.NroSocio}");
        Console.WriteLine($"Email:     {socio.Email}");
        Console.WriteLine($"Categoría: {socio.IdTipoSocioNavigation.Descripcion}");

        string estadoTexto = socio.Activo == 1 ? "Activo" : "Inactivo";
        Console.WriteLine($"Estado:    {estadoTexto}");

        var prestamosActivos = socio.Prestamos.Where(p => p.FechaDevolucion == null).ToList();
        Console.WriteLine($"\n--- Préstamos Activos ({prestamosActivos.Count}) ---");
        if (prestamosActivos.Count == 0)
        {
            Console.WriteLine("No tiene préstamos activos.");
        }
        else
        {
            foreach (var p in prestamosActivos)
            {
                Console.WriteLine($"- {p.IsbnNavigation.Titulo} | Retiró: {p.FechaPrestamo} | Vence: {p.FechaVencimiento}");
            }
        }

        var historial = socio.Prestamos.Where(p => p.FechaDevolucion != null).ToList();
        Console.WriteLine($"\n--- Historial de Devoluciones ({historial.Count}) ---");
        if (historial.Count == 0)
        {
            Console.WriteLine("No tiene devoluciones registradas.");
        }
        else
        {
            foreach (var p in historial)
            {
                Console.WriteLine($"- {p.IsbnNavigation.Titulo} | Entregado el: {p.FechaDevolucion}");
            }
        }

        var multasPendientes = socio.Multas.Where(m => m.Abonada == 0).ToList();
        Console.WriteLine($"\n--- Multas Pendientes ({multasPendientes.Count}) ---");
        if (multasPendientes.Count == 0)
        {
            Console.WriteLine("No tiene multas pendientes de pago.");
        }
        else
        {
            foreach (var m in multasPendientes)
            {
                Console.WriteLine($"- Monto: ${m.Monto} | Demora: {m.DiasDemora} días | Generada el: {m.FechaGeneracion}");
            }
        }
    }
}
