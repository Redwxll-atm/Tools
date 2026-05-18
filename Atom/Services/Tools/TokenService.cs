using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atom.Services.Tools
{
    public static class TokenService
    {
        public static void HandleTokenFormatter()
        {
            Console.WriteLine("=== TOKEN FORMATTER ===");
            Console.WriteLine("Format supporté: email:pass:token -> token");
            Console.Write("Chemin du fichier tokens: ");
            string? path = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Console.WriteLine("[!] Fichier introuvable.");
                return;
            }

            var lines = File.ReadAllLines(path);
            var formatted = new List<string>();

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length >= 3)
                {
                    // On prend le dernier élément comme étant potentiellement le token
                    formatted.Add(parts.Last());
                }
                else
                {
                    formatted.Add(line);
                }
            }

            File.WriteAllLines("formatted_tokens.txt", formatted);
            Console.WriteLine($"[+] {formatted.Count} tokens formatés dans formatted_tokens.txt");
        }

        public static void HandleTokenSorter()
        {
            Console.WriteLine("=== TOKEN SORTER ===");
            Console.Write("Chemin du fichier tokens: ");
            string? path = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            var lines = File.ReadAllLines(path).ToList();
            lines.Sort();

            File.WriteAllLines("sorted_tokens.txt", lines);
            Console.WriteLine($"[+] Tokens triés dans sorted_tokens.txt");
        }

        public static void HandleRemoveDuplicates()
        {
            Console.WriteLine("=== REMOVE DUPLICATES ===");
            Console.Write("Chemin du fichier: ");
            string? path = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            var lines = File.ReadAllLines(path).Distinct().ToList();

            File.WriteAllLines("cleaned_file.txt", lines);
            Console.WriteLine($"[+] Doublons supprimés. Résultat dans cleaned_file.txt");
        }

        public static async Task HandleTokenInfo()
        {
            // Réutilisation de la logique de DiscordService mais adaptée pour l'info détaillée
            await DiscordService.HandleTokenChecker();
        }

        public static async Task HandleNitroChecker()
        {
            Console.WriteLine("=== NITRO GIFT CHECKER ===");
            Console.Write("Entrez le lien Nitro (ex: discord.gift/abc): ");
            string? code = Console.ReadLine()?.Trim().Split('/').Last();
            
            if (string.IsNullOrEmpty(code)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            try
            {
                var response = await client.GetAsync($"https://discord.com/api/v10/entitlements/gift-codes/{code}");
                if (response.IsSuccessStatusCode)
                    Console.WriteLine("[+] Cadeau Nitro VALIDE !");
                else
                    Console.WriteLine($"[-] Cadeau Nitro Invalide ou déjà réclamé. ({response.StatusCode})");
            }
            catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
        }

        public static async Task HandleStatusRotator()
        {
            Console.WriteLine("=== TOKEN STATUS ROTATOR ===");
            Console.Write("Entrez le Token : ");
            string? token = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            Console.WriteLine("Entrez les statuts (un par ligne, ligne vide pour terminer) :");
            var statuses = new List<string>();
            while (true)
            {
                string? s = Console.ReadLine();
                if (string.IsNullOrEmpty(s)) break;
                statuses.Add(s);
            }

            if (statuses.Count == 0) return;

            Console.WriteLine("[*] Rotation démarrée. CTRL+C pour arrêter.");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            int i = 0;
            while (true)
            {
                try
                {
                    // For User accounts, the custom status is often in settings or profile
                    var payload = new { custom_status = new { text = statuses[i] } };
                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PatchAsync("https://discord.com/api/v10/users/@me/settings", content);
                    
                    if (response.IsSuccessStatusCode)
                        Console.WriteLine($"[+] Statut mis à jour : {statuses[i]}");
                    else
                        Console.WriteLine($"[-] Erreur ({response.StatusCode}): {await response.Content.ReadAsStringAsync()}");

                    i = (i + 1) % statuses.Count;
                    await Task.Delay(10000); // 10 secondes entre chaque rotation
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine($"[!] Erreur : {ex.Message}"); 
                    break; 
                }
            }
        }
    }
}
