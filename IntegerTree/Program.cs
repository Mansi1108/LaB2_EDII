using API.Models;
using ClassLibrary1;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IntegerTree
{
    class Program
    {
        static void Main(string[] args)
        {
        Begginnig:
            Console.WriteLine("Por favor ingrese el grado del árbol");
            try
            {
                int order = Convert.ToInt32(Console.ReadLine());
                BTree<ModifiedInts> BTree = new BTree<ModifiedInts>(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), order);
                Console.WriteLine($"{Environment.NewLine}Se ha creado un árbol de grado {order}");
                bool HasMoreValues = true;
                bool NotDone = true;
                bool MoreToEliminate = true;
                do
                {
                    Console.WriteLine($"{Environment.NewLine}Por favor ingrese el número de la operción deseada: 1)Insertar 2)Eliminar 3)Recorridos 4)Salir.");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            do
                            {
                            Insert:
                                Console.WriteLine($"{Environment.NewLine}Por favor ingrese los valores a insertar en el árbol, separados por comas.");
                                try
                                {

                                    var intList = new List<ModifiedInts>();
                                    var line = Console.ReadLine().Split(',');
                                    foreach (var value in line)
                                    {
                                        intList.Add(new ModifiedInts(Convert.ToInt32(value)));
                                    }
                                    foreach (var item in intList)
                                    {
                                        BTree.AddValue(item);
                                    }
                                    Console.WriteLine("¿Quisieras insertar más valores? | Presione 'Y'. De lo contrario, presione cualquier otra tecla.");
                                    if (Console.ReadKey().Key != ConsoleKey.Y)
                                    {
                                        HasMoreValues = false;
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("Por favor ingrese un valor válido");
                                    Console.ReadLine();
                                    goto Insert;
                                }
                            } while (HasMoreValues);
                            break;
                        case "2":
                            do
                            {
                            Delete:
                                Console.WriteLine($"{Environment.NewLine}Por favor ingrese el valor que desea eliminar");
                                try
                                {
                                    var intList = new List<ModifiedInts>();
                                    var line = Console.ReadLine().Split(',');
                                    foreach (var value in line)
                                    {
                                        intList.Add(new ModifiedInts(Convert.ToInt32(value)));
                                    }
                                    foreach (var item in intList)
                                    {
                                        BTree.DeleteValue(item);
                                    }
                                    if (BTree.DeleteValue(new ModifiedInts(int.Parse(Console.ReadLine()))))
                                    {
                                        Console.WriteLine($"{Environment.NewLine}El valor fue eliminado");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{Environment.NewLine}El valor no se encuentra en el árbol");
                                    }
                                    Console.WriteLine("¿Quisieras eliminar más valores? | Presione 'Y'. De lo contrario, presione cualquier otra tecla.");
                                    if (Console.ReadKey().Key != ConsoleKey.Y)
                                    {
                                        MoreToEliminate = false;
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("Por favor ingrese un valor válido");
                                    Console.ReadLine();
                                    goto Delete;
                                }
                            } while (MoreToEliminate);
                            break;
                        case "3":
                            bool Pathings = true;
                            do
                            {
                            Order:
                                List<ModifiedInts> Path = new List<ModifiedInts>();
                                Console.Clear();
                                Console.WriteLine("Por favor ingrese el número correspondiente al tipo de recorrido que desea realizar en el árbol");
                                Console.WriteLine("1. PreOrder");
                                Console.WriteLine("2. InOrder");
                                Console.WriteLine("3. PostOrder");
                                try
                                {
                                    switch (Convert.ToInt32(Console.ReadLine()))
                                    {
                                        case 1:

                                            Path = BTree.Pathing(1);
                                            break;
                                        case 2:
                                            Path = BTree.Pathing(2);

                                            break;
                                        case 3:
                                            Path = BTree.Pathing(3);

                                            break;
                                    }
                                    if (Path != null)
                                    {
                                        foreach (var item in Path)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine(item.Number.ToString());
                                        }
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("Por favor ingrese un recorrido válido");
                                    Console.ReadLine();
                                    goto Order;
                                    throw;
                                }
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("¿Quisieras ordenarla de otra manera? | Presione 'Y'. De lo contrario, presione cualquier otra tecla.");
                                if (Console.ReadKey().Key != ConsoleKey.Y)
                                {
                                    Pathings = false;
                                }
                            }
                            while (Pathings);
                            break;
                        case "4":
                            NotDone = false;
                            Console.WriteLine($"{Environment.NewLine}¡Tenga un feliz día!");
                            break;
                    }
                } while (NotDone);                 
            }
            catch
            {
                Console.WriteLine("Por favor ingrese un grado válido.");
                Console.ReadLine();
                Console.Clear();
                goto Begginnig;
            }
        }
    }           
}
