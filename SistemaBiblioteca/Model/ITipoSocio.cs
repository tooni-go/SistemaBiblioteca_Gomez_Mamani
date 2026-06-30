using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public interface ITipoSocio
{
    int IdTipoSocio { get; set; }
    string Descripcion { get; set; }
    int MaxLibrosSimultaneos { get; set; }
    int DiasPrestamo { get; set; }
    decimal MultaPorDia { get; set; }
    ICollection<Socio> Socios { get; set; }
}
