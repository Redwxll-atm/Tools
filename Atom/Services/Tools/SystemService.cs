using System;
using System.Management;
using Microsoft.Win32;
using System.Linq;

namespace Atom.Services.Tools
{
    public static class SystemService
    {
        public static void HandleSerialChecker()
        {
            Console.WriteLine("=== SERIAL CHECKER ===");
            try
            {
                Console.WriteLine($"[Disk Serial] {GetWmiValue("Win32_DiskDrive", "SerialNumber")}");
                Console.WriteLine($"[BIOS Serial] {GetWmiValue("Win32_BIOS", "SerialNumber")}");
                Console.WriteLine($"[Mobo Serial] {GetWmiValue("Win32_BaseBoard", "SerialNumber")}");
                Console.WriteLine($"[CPU ID]      {GetWmiValue("Win32_Processor", "ProcessorId")}");
                Console.WriteLine($"[HWID]        {GetHardwareId()}");
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
            using var rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
            return rk?.GetValue("MachineGuid")?.ToString() ?? "N/A";
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
