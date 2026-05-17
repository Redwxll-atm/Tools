using System;
using System.Management;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Forms;

namespace Atom.Services.Tools
{
    public static class SystemService
    {
        public static void HandleHWIDTool()
        {
            Console.WriteLine("=== YOUR HARDWARE ID (HWID) ===");
            string hwid = GetHardwareId();
            Console.WriteLine($"\n[>] HWID: {hwid}");
            
            Console.WriteLine("\n[*] Voulez-vous copier le HWID dans le presse-papier ? (O/N)");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.O)
            {
                try
                {
                    // Using a Thread because Clipboard requires STA mode
                    var thread = new System.Threading.Thread(() => Clipboard.SetText(hwid));
                    thread.SetApartmentState(System.Threading.ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    Console.WriteLine("[+] HWID copié avec succès !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Erreur lors de la copie : {ex.Message}");
                }
            }
        }

        public static void HandleSerialChecker()
        {
            Console.WriteLine("=== SERIAL CHECKER ===");
            try
            {
                Console.WriteLine($"[Disk Serial] {GetWmiValue("Win32_DiskDrive", "SerialNumber")}");
                Console.WriteLine($"[BIOS Serial] {GetWmiValue("Win32_BIOS", "SerialNumber")}");
                Console.WriteLine($"[Mobo Serial] {GetWmiValue("Win32_BaseBoard", "SerialNumber")}");
                Console.WriteLine($"[CPU ID]      {GetWmiValue("Win32_Processor", "ProcessorId")}");
                Console.WriteLine($"[HWID Registry] {GetHardwareId()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur: {ex.Message}");
            }
        }

        public static void HandleSerialChanger()
        {
            Console.WriteLine("=== SERIAL CHANGER (HWID SPOOFER) ===");
            Console.WriteLine("[!] Attention: Cette opération modifie des clés de registre.");
            Console.WriteLine("[*] Modification du HWID...");
            
            string newHwid = Guid.NewGuid().ToString();
            if (SetRegistryValue(@"SOFTWARE\Microsoft\Cryptography", "MachineGuid", newHwid))
            {
                Console.WriteLine($"[+] Nouveau HWID défini: {newHwid}");
            }

            Console.WriteLine("[*] Modification du ProductID...");
            string newProdId = GenerateRandomId(5) + "-" + GenerateRandomId(5) + "-" + GenerateRandomId(5) + "-" + GenerateRandomId(5);
            if (SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductId", newProdId))
            {
                Console.WriteLine($"[+] Nouveau ProductID défini: {newProdId}");
            }

            Console.WriteLine("\n[!] Certains changements nécessitent un redémarrage.");
        }

        private static string GetWmiValue(string table, string property)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {table}");
                foreach (var obj in searcher.Get())
                {
                    return obj[property]?.ToString()?.Trim() ?? "N/A";
                }
            }
            catch { }
            return "N/A";
        }

        private static string GetHardwareId()
        {
            try
            {
                using var rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                return rk?.GetValue("MachineGuid")?.ToString() ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        private static bool SetRegistryValue(string path, string name, string value)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(path, true);
                if (key != null)
                {
                    key.SetValue(name, value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur registre ({name}): {ex.Message}");
            }
            return false;
        }

        private static string GenerateRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
