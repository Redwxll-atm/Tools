using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atom.Services.Tools
{
    public static class NetworkService
    {
        public static async Task HandleIpLookup()
        {
            Console.WriteLine("=== IP LOOKUP ===");
            Console.Write("Entrez l'IP (laissez vide pour la vôtre): ");
            string? ip = Console.ReadLine()?.Trim();

            using var client = new HttpClient();
            try
            {
                string url = string.IsNullOrEmpty(ip) ? "http://ip-api.com/json/" : $"http://ip-api.com/json/{ip}";
                var response = await client.GetStringAsync(url);
                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (root.GetProperty("status").GetString() == "success")
                {
                    Console.WriteLine($"\n[+] Infos pour {root.GetProperty("query").GetString()}:");
                    Console.WriteLine($"Pays    : {root.GetProperty("country").GetString()}");
                    Console.WriteLine($"Ville   : {root.GetProperty("city").GetString()}");
                    Console.WriteLine($"ISP     : {root.GetProperty("isp").GetString()}");
                    Console.WriteLine($"Lat/Lon : {root.GetProperty("lat").GetDouble()} / {root.GetProperty("lon").GetDouble()}");
                }
                else
                {
                    Console.WriteLine("[!] Échec de la récupération des infos.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur: {ex.Message}");
            }
        }
    }
}
