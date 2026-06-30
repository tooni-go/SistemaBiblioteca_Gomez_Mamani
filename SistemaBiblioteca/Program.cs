using SistemaBiblioteca.Model;
using SistemaBiblioteca.UI;

namespace SistemaBiblioteca;

internal class Program
{
    static void Main(string[] args)
    {
        using ISistemaBibliotecaContext context = new SistemaBibliotecaContext();
        IMenuConsola menu = new MenuConsola(context);
        menu.Ejecutar();
    }
}
