using SistemaBiblioteca.Model;
using SistemaBiblioteca.UI;

namespace SistemaBiblioteca;

internal class Program
{
    static void Main(string[] args)
    {
        using var context = new SistemaBibliotecaContext();
        var menu = new MenuConsola(context);
        menu.Ejecutar();
    }
}
