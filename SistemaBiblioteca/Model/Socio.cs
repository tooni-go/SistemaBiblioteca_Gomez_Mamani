using System;
using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public partial class Socio : ISocio
{
    public int NroSocio { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int IdTipoSocio { get; set; }

    public int Activo { get; set; }

    public virtual TipoSocio IdTipoSocioNavigation { get; set; } = null!;

    public virtual ICollection<Multa> Multas { get; set; } = new List<Multa>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
