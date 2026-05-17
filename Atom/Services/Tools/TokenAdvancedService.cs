using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atom.Services.Tools
{
    public static class TokenAdvancedService
    {
        public static async Task HandlePaymentChecker()
        {
            Console.WriteLine("=== TOKEN PAYMENT CHECKER ===");
            Console.Write("Entrez le Token : ");
            string? token = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);

            try
            {
                var response = await client.GetAsync("https://discord.com/api/v10/users/@me/billing/payment-sources");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    int count = doc.RootElement.GetArrayLength();
                    Console.WriteLine($"[+] Méthodes de paiement trouvées : {count}");
                }
                else Console.WriteLine($"[-] Erreur: {response.StatusCode}");
            }
            catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
        }

        public static async Task HandleGuildTools()
        {
            Console.WriteLine("=== TOKEN GUILD TOOLS ===");
            Console.Write("Entrez le Token : ");
            string? token = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            var options = new System.Collections.Generic.List<string> { "Lister Serveurs", "Quitter tous les serveurs", "Retour" };
            int choice = Atom.Utils.UIHelper.SingleChoice(options);
            Console.Clear();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);

            if (choice == 0 || choice == 1)
            {
                try
                {
                    var response = await client.GetAsync("https://discord.com/api/v10/users/@me/guilds");
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(json);
                        var guilds = doc.RootElement.EnumerateArray();
                        
                        if (choice == 0)
                        {
                            foreach (var g in guilds)
                                Console.WriteLine($"- {g.GetProperty("name").GetString()} ({g.GetProperty("id").GetString()})");
                        }
                        else
                        {
                            Console.Write("VOULEZ-VOUS VRAIMENT QUITTER TOUS LES SERVEURS ? (o/n): ");
                            if (Console.ReadLine()?.ToLower() != "o") return;

                            foreach (var g in guilds)
                            {
                                string id = g.GetProperty("id").GetString()!;
                                var delResponse = await client.DeleteAsync($"https://discord.com/api/v10/users/@me/guilds/{id}");
                                Console.WriteLine($"[{(delResponse.IsSuccessStatusCode ? "+" : "-")}] Quitté: {g.GetProperty("name").GetString()}");
                                await Task.Delay(1000);
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
            }
        }
    }
}
