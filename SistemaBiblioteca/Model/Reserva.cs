using System;
using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int NroSocio { get; set; }

    public string Isbn { get; set; } = null!;

    public string FechaReserva { get; set; } = null!;

    public int IdEstado { get; set; }

    public virtual Estado IdEstadoNavigation { get; set; } = null!;

    public virtual Libro IsbnNavigation { get; set; } = null!;

    public virtual Socio NroSocioNavigation { get; set; } = null!;
}
