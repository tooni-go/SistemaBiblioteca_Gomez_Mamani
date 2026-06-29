using System;
using System.Collections.Generic;

namespace SistemaBiblioteca.Model;

public partial class TipoSocio
{
    public int IdTipoSocio { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Socio> Socios { get; set; } = new List<Socio>();
}
