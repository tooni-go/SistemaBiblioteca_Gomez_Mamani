namespace SistemaBiblioteca.Model;

public interface IReserva
{
    int IdReserva { get; set; }
    int NroSocio { get; set; }
    string Isbn { get; set; }
    string FechaReserva { get; set; }
    int IdEstado { get; set; }
    Estado IdEstadoNavigation { get; set; }
    Libro IsbnNavigation { get; set; }
    Socio NroSocioNavigation { get; set; }
}
