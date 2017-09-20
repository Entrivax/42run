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
            new MainWindow().Run(60);
        }
    }
}
