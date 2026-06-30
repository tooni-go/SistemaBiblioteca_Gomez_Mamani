using System;
using SistemaBiblioteca.Model;

namespace SistemaBiblioteca.Services;

public interface IReglasNegocioService
{
    string? ValidarSocioParaPrestamo(Socio socio);
    bool TieneMultasPendientes(int nroSocio);
    int ObtenerCopiasDisponibles(string isbn);
    bool HayCopiasDisponibles(string isbn);
    int ContarPrestamosActivos(int nroSocio);
    bool PuedeTomarMasLibros(Socio socio);
    DateOnly CalcularFechaVencimiento(DateOnly fechaPrestamo, TipoSocio tipoSocio);
    Socio? ObtenerSocioConTipo(int nroSocio);
}
