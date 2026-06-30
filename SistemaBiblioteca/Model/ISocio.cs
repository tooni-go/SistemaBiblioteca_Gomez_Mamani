using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public interface ISocio
{
    int NroSocio { get; set; }
    string Nombre { get; set; }
    string Apellido { get; set; }
    string Email { get; set; }
    int IdTipoSocio { get; set; }
    int Activo { get; set; }
    TipoSocio IdTipoSocioNavigation { get; set; }
    ICollection<Multa> Multas { get; set; }
    ICollection<Prestamo> Prestamos { get; set; }
    ICollection<Reserva> Reservas { get; set; }
}
