using System;
using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public interface IPrestamo
{
    int IdPrestamo { get; set; }
    int NroSocio { get; set; }
    string Isbn { get; set; }
    DateOnly FechaPrestamo { get; set; }
    DateOnly FechaVencimiento { get; set; }
    DateOnly? FechaDevolucion { get; set; }
    int IdEstado { get; set; }
    Estado IdEstadoNavigation { get; set; }
    Libro IsbnNavigation { get; set; }
    Socio NroSocioNavigation { get; set; }
    ICollection<Multa> Multas { get; set; }
}
