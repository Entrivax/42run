using System;

namespace _42run
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                new MainWindow().Run(60);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Une exception de type {exception.GetType()} est survenue, message : {exception.Message}");
                Console.Error.WriteLine($"Stacktrace:");
                Console.Error.WriteLine(exception.StackTrace);
                Console.WriteLine("Sortie...");
                Environment.Exit(1);
            }
        }
    }
}
