using System;
using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public partial class Prestamo
{
    public int IdPrestamo { get; set; }

    public int NroSocio { get; set; }

    public string Isbn { get; set; } = null!;

    public DateOnly FechaPrestamo { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public DateOnly? FechaDevolucion { get; set; }

    public int IdEstado { get; set; }

    public virtual Estado IdEstadoNavigation { get; set; } = null!;

    public virtual Libro IsbnNavigation { get; set; } = null!;

    public virtual Socio NroSocioNavigation { get; set; } = null!;

    public virtual ICollection<Multa> Multas { get; set; } = new List<Multa>();
}
