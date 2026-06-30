using System;

namespace SistemaBiblioteca.Model;

public interface IMulta
{
    int IdMulta { get; set; }
    int NroSocio { get; set; }
    int IdPrestamo { get; set; }
    decimal Monto { get; set; }
    int DiasDemora { get; set; }
    DateOnly FechaGeneracion { get; set; }
    int Abonada { get; set; }
    Prestamo IdPrestamoNavigation { get; set; }
    Socio NroSocioNavigation { get; set; }
}
