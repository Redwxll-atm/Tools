using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atom.Services.Tools
{
    public static class WebhookService
    {
        public static async Task HandleWebhookMenu()
        {
            var options = new System.Collections.Generic.List<string> 
            { 
                "Check Webhook Info", 
                "Delete Webhook", 
                "Send Message (Simple)", 
                "Retour" 
            };
            
            int choice = Atom.Utils.UIHelper.SingleChoice(options);
            Console.Clear();

            switch (choice)
            {
                case 0: await CheckWebhookInfo(); break;
                case 1: await DeleteWebhook(); break;
                case 2: await DiscordService.HandleDiscordWebhook(); break;
            }
        }

        private static async Task CheckWebhookInfo()
        {
            Console.WriteLine("=== WEBHOOK INFO ===");
            Console.Write("URL du Webhook: ");
            string? url = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(url)) return;

            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    Console.WriteLine($"\n[+] Infos du Webhook:");
                    Console.WriteLine($"Nom      : {root.GetProperty("name").GetString()}");
                    Console.WriteLine($"ID       : {root.GetProperty("id").GetString()}");
                    Console.WriteLine($"Channel  : {root.GetProperty("channel_id").GetString()}");
                    Console.WriteLine($"Guild    : {root.GetProperty("guild_id").GetString()}");
                    if (root.TryGetProperty("token", out var token))
                        Console.WriteLine($"Token    : {token.GetString()}");
                }
                else
                {
                    Console.WriteLine($"[-] Erreur: {response.StatusCode}");
                }
            }
            catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
            
            Console.WriteLine("\nAppuyez sur une touche...");
            Console.ReadKey();
        }

        private static async Task DeleteWebhook()
        {
            Console.WriteLine("=== DELETE WEBHOOK ===");
            Console.Write("URL du Webhook à supprimer: ");
            string? url = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(url)) return;

            Console.Write("Êtes-vous sûr ? (o/n): ");
            if (Console.ReadLine()?.ToLower() != "o") return;

            using var client = new HttpClient();
            try
            {
                var response = await client.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                    Console.WriteLine("[+] Webhook supprimé avec succès.");
                else
                    Console.WriteLine($"[-] Échec de la suppression: {response.StatusCode}");
            }
            catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }

            Console.WriteLine("\nAppuyez sur une touche...");
            Console.ReadKey();
        }
    }
}
