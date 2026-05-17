using System;
using System.IO;
using System.Diagnostics;

namespace Atom.Services.Tools
{
    public static class CleanerService
    {
        public static void HandleCleanerMenu()
        {
            var options = new System.Collections.Generic.List<string> 
            { 
                "Clean Crash Dumps", 
                "Clean Temporary Files", 
                "Clean Prefetch", 
                "Retour" 
            };
            
            int choice = Atom.Utils.UIHelper.SingleChoice(options);
            Console.Clear();

            switch (choice)
            {
                case 0: CleanCrashDumps(); break;
                case 1: CleanTempFiles(); break;
                case 2: CleanPrefetch(); break;
            }
            if (choice != 3) { Console.WriteLine("\nAppuyez sur une touche..."); Console.ReadKey(); }
        }

        private static void CleanCrashDumps()
        {
            Console.WriteLine("=== CLEAN CRASH DUMPS ===");
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps");
            DeleteDirectoryContent(path);
        }

        private static void CleanTempFiles()
        {
            Console.WriteLine("=== CLEAN TEMPORARY FILES ===");
            DeleteDirectoryContent(Path.GetTempPath());
            string windowsTemp = Path.Combine(Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows", "Temp");
            DeleteDirectoryContent(windowsTemp);
        }

        private static void CleanPrefetch()
        {
            Console.WriteLine("=== CLEAN PREFETCH ===");
            string path = Path.Combine(Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows", "Prefetch");
            DeleteDirectoryContent(path);
        }

        private static void DeleteDirectoryContent(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"[!] Dossier introuvable : {path}");
                return;
            }

            Console.WriteLine($"[*] Nettoyage de : {path}");
            int count = 0;
            
            foreach (string file in Directory.GetFiles(path))
            {
                try { File.Delete(file); count++; } catch { }
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                try { Directory.Delete(dir, true); count++; } catch { }
            }

            Console.WriteLine($"[+] {count} éléments supprimés.");
        }
    }
}
