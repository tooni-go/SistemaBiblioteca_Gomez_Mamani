namespace SistemaBiblioteca.Services;

public interface IReporteService
{
    void LibrosMasPrestados();
    void SociosConMultasPendientes();
    void PrestamosVencidos();
    void DisponibilidadLibro();
    void HistorialSocio();
}
