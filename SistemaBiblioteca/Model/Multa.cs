namespace SistemaBiblioteca.Model;

public partial class Multa : IMulta
{
    public int IdMulta { get; set; }

    public int NroSocio { get; set; }

    public int IdPrestamo { get; set; }

    public decimal Monto { get; set; }

    public int DiasDemora { get; set; }

    public DateOnly FechaGeneracion { get; set; }

    public int Abonada { get; set; }

    public virtual Prestamo IdPrestamoNavigation { get; set; } = null!;

    public virtual Socio NroSocioNavigation { get; set; } = null!;
}
