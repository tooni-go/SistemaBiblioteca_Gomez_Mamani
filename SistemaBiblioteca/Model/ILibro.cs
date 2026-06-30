using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public interface ILibro
{
    string Isbn { get; set; }
    string Titulo { get; set; }
    string Autor { get; set; }
    string Genero { get; set; }
    int CantidadCopias { get; set; }
    ICollection<Prestamo> Prestamos { get; set; }
    ICollection<Reserva> Reservas { get; set; }
}
