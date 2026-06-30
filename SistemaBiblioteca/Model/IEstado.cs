using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public interface IEstado
{
    int IdEstado { get; set; }
    string Descripcion { get; set; }
    ICollection<Prestamo> Prestamos { get; set; }
    ICollection<Reserva> Reservas { get; set; }
}
