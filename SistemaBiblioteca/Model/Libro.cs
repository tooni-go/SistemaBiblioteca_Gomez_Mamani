using System;
using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public partial class Libro
{
    public string Isbn { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string Autor { get; set; } = null!;

    public string Genero { get; set; } = null!;

    public int CantidadCopias { get; set; }

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
