namespace SistemaBiblioteca.Services;

public interface IPrestamoService
{
    void RealizarPrestamo();
    void RegistrarDevolucion();
    void RegistrarReserva();
    void VerDetalleSocio();
}
