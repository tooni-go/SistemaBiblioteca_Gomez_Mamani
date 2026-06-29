using SistemaBiblioteca.Model;
using System;
using System.Linq;

namespace SistemaBiblioteca
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new SistemaBibliotecaContext();

            Console.WriteLine("=============================================");
            Console.WriteLine("    SISTEMA DE BIBLIOTECA - LIBROS DISPONIBLES");
            Console.WriteLine("=============================================\n");

            var libros = context.Libros.ToList();

            if (libros.Any())
            {
                foreach (var libro in libros)
                {
                    Console.WriteLine($"[ISBN: {libro.Isbn}] {libro.Titulo}");
                    Console.WriteLine($"   Autor:  {libro.Autor}");
                    Console.WriteLine($"   Género: {libro.Genero}");
                    Console.WriteLine($"   Copias: {libro.CantidadCopias}");
                    Console.WriteLine("---------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("No hay libros registrados en la biblioteca.");
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadLine();
        }
    }
}