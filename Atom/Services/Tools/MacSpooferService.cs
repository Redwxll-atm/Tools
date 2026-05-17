using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;

namespace Atom.Services.Tools
{
    public static class MacSpooferService
    {
        public static void HandleMacSpoofer()
        {
            Console.WriteLine("=== MAC ADDRESS SPOOFER ===");
            
            var adapters = GetNetworkAdapters();
            if (adapters.Count == 0)
            {
                Console.WriteLine("[!] Aucun adaptateur réseau trouvé.");
                return;
            }

            for (int i = 0; i < adapters.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {adapters[i].Description} (MAC actuelle: {adapters[i].MacAddress})");
            }

            Console.Write("\nSélectionnez un adaptateur (1-{0}): ", adapters.Count);
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > adapters.Count)
            {
                Console.WriteLine("[!] Sélection invalide.");
                return;
            }

            var selectedAdapter = adapters[choice - 1];
            
            Console.WriteLine("\n1. MAC Aléatoire");
            Console.WriteLine("2. MAC Personnalisée");
            Console.WriteLine("3. Réinitialiser (MAC d'origine)");
            Console.Write("Choix: ");
            string subChoice = Console.ReadLine();

            string newMac = "";
            switch (subChoice)
            {
                case "1":
                    newMac = GenerateRandomMac();
                    break;
                case "2":
                    Console.Write("Entrez la nouvelle MAC (ex: 001122334455): ");
                    newMac = Console.ReadLine().Replace("-", "").Replace(":", "").ToUpper();
                    break;
                case "3":
                    newMac = null; // Supprimer la clé de registre pour réinitialiser
                    break;
                default:
                    Console.WriteLine("[!] Choix invalide.");
                    return;
            }

            if (SetMacAddress(selectedAdapter.Id, newMac))
            {
                Console.WriteLine("[+] MAC mise à jour dans le registre.");
                Console.WriteLine("[*] Redémarrage de l'adaptateur...");
                RestartAdapter(selectedAdapter.Name);
                Console.WriteLine("[+] Opération terminée.");
            }
            else
            {
                Console.WriteLine("[!] Échec de la mise à jour de la MAC.");
            }
        }

        private static List<NetworkAdapterInfo> GetNetworkAdapters()
        {
            var adapters = new List<NetworkAdapterInfo>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    adapters.Add(new NetworkAdapterInfo
                    {
                        Id = ni.Id,
                        Name = ni.Name,
                        Description = ni.Description,
                        MacAddress = ni.GetPhysicalAddress().ToString()
                    });
                }
            }
            return adapters;
        }

        private static string GenerateRandomMac()
        {
            Random random = new Random();
            byte[] buffer = new byte[6];
            random.NextBytes(buffer);
            buffer[0] = (byte)((buffer[0] & 0xFE) | 0x02); // Set locally administered bit
            return string.Concat(buffer.Select(b => b.ToString("X2")));
        }

        private static bool SetMacAddress(string adapterId, string newMac)
        {
            try
            {
                string keyPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";
                using (RegistryKey baseKey = Registry.LocalMachine.OpenSubKey(keyPath, true))
                {
                    foreach (string subKeyName in baseKey.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = baseKey.OpenSubKey(subKeyName, true))
                        {
                            if (subKey.GetValue("NetCfgInstanceId")?.ToString() == adapterId)
                            {
                                if (string.IsNullOrEmpty(newMac))
                                    subKey.DeleteValue("NetworkAddress", false);
                                else
                                    subKey.SetValue("NetworkAddress", newMac, RegistryValueKind.String);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur Registre: {ex.Message}");
            }
            return false;
        }

        private static void RestartAdapter(string adapterName)
        {
            try
            {
                ProcessStartInfo psiDisable = new ProcessStartInfo("netsh", $"interface set interface \"{adapterName}\" disable")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Verb = "runas"
                };
                Process.Start(psiDisable)?.WaitForExit();

                ProcessStartInfo psiEnable = new ProcessStartInfo("netsh", $"interface set interface \"{adapterName}\" enable")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Verb = "runas"
                };
                Process.Start(psiEnable)?.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur lors du redémarrage de l'adaptateur: {ex.Message}");
            }
        }

        private class NetworkAdapterInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string MacAddress { get; set; }
        }
    }
}
