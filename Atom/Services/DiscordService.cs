using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Atom.Models;
using Atom.Utils;

namespace Atom.Services
{
    public static class DiscordService
    {
        public static async Task<bool> HandleTokenChecker(string? providedToken = null)
        {
            string? token = providedToken;
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("\n--- Discord Token Checker ---");
                Console.Write("Entrez le Token Discord : ");
                token = Console.ReadLine()?.Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Token invalide.");
                return false;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            try
            {
                var response = await client.GetAsync("https://discord.com/api/v10/users/@me");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);
                    var root = doc.RootElement;

                    string username = root.GetProperty("username").GetString() ?? "Inconnu";
                    string id = root.GetProperty("id").GetString() ?? "Inconnu";
                    
                    int premiumType = 0;
                    if (root.TryGetProperty("premium_type", out var premiumProp))
                        premiumType = premiumProp.GetInt32();

                    string nitroStatus = premiumType switch
                    {
                        1 => "Nitro Classic",
                        2 => "Nitro Boost",
                        3 => "Nitro Basic",
                        _ => "Aucun"
                    };

                    int publicFlags = 0;
                    if (root.TryGetProperty("public_flags", out var flagsProp))
                        publicFlags = flagsProp.GetInt32();

                    var badges = GetBadges(publicFlags);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n[+] Token Valide !");
                    Console.ResetColor();
                    Console.WriteLine($"Pseudo  : {username}");
                    Console.WriteLine($"ID      : {id}");
                    Console.WriteLine($"Nitro   : {nitroStatus}");
                    Console.WriteLine($"Badges  : {(badges.Count > 0 ? string.Join(", ", badges) : "Aucun")}");
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[-] Token Invalide ou erreur API (Code: {response.StatusCode})");
                    Console.ResetColor();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                return false;
            }
        }

        public static async Task HandleTokenEditorMenu()
        {
            Console.Write("Entrez le Token Discord : ");
            string? token = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(token)) return;

            var options = new List<string> { "Changer Bio", "Changer Username", "Changer Pronoms", "Retour" };
            int choice = UIHelper.SingleChoice(options);
            Console.Clear();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            string url = "https://discord.com/api/v10/users/@me";
            object? payload = null;

            switch (choice)
            {
                case 0:
                    url = "https://discord.com/api/v10/users/@me/profile";
                    Console.Write("Nouvelle Bio : ");
                    payload = new { bio = Console.ReadLine() };
                    break;
                case 1:
                    Console.Write("Nouveau Username : ");
                    payload = new { username = Console.ReadLine() };
                    break;
                case 2:
                    url = "https://discord.com/api/v10/users/@me/profile";
                    Console.Write("Nouveaux Pronoms : ");
                    payload = new { pronouns = Console.ReadLine() };
                    break;
                default: return;
            }

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync(url, content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("[+] Profil mis à jour !");
            else
            {
                string errorMsg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[-] Erreur: {response.StatusCode}");
                Console.WriteLine($"Détails: {errorMsg}");
            }
            
            Console.WriteLine("\nAppuyez sur une touche...");
            Console.ReadKey();
        }

        public static async Task HandleAvatarScraper()
        {
            Console.Write("Entrez l'ID de l'utilisateur : ");
            string? userId = Console.ReadLine()?.Trim();
            Console.Write("Entrez votre Token (pour l'API) : ");
            string? token = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            try
            {
                var response = await client.GetAsync($"https://discord.com/api/v10/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    
                    if (doc.RootElement.TryGetProperty("avatar", out var avatarProp) && avatarProp.ValueKind != JsonValueKind.Null)
                    {
                        string avatar = avatarProp.GetString()!;
                        string avatarUrl = $"https://cdn.discordapp.com/avatars/{userId}/{avatar}.png?size=1024";
                        Console.WriteLine($"[+] Avatar URL: {avatarUrl}");
                    }
                    else Console.WriteLine("[-] Cet utilisateur n'a pas d'avatar.");
                }
                else
                {
                    Console.WriteLine($"[-] Erreur API: {response.StatusCode}");
                }
            }
            catch (Exception ex) { Console.WriteLine($"[!] Erreur: {ex.Message}"); }
            
            Console.WriteLine("\nAppuyez sur une touche...");
            Console.ReadKey();
        }

        private static List<string> GetBadges(int flags)
        {
            var badges = new List<string>();
            if ((flags & 1) != 0) badges.Add("Discord Employee");
            if ((flags & 2) != 0) badges.Add("Partnered Server Owner");
            if ((flags & 4) != 0) badges.Add("HypeSquad Events");
            if ((flags & 8) != 0) badges.Add("Bug Hunter Level 1");
            if ((flags & 64) != 0) badges.Add("House Bravery");
            if ((flags & 128) != 0) badges.Add("House Brilliance");
            if ((flags & 256) != 0) badges.Add("House Balance");
            if ((flags & 512) != 0) badges.Add("Early Supporter");
            if ((flags & 16384) != 0) badges.Add("Bug Hunter Level 2");
            if ((flags & 131072) != 0) badges.Add("Verified Developer");
            if ((flags & 4194304) != 0) badges.Add("Active Developer");
            return badges;
        }

        public static async Task HandleDiscordWebhook()
        {
            Console.WriteLine("\n--- Discord Webhook Tool ---");
            Console.Write("Entrez l'URL du Webhook : ");
            string? url = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(url)) return;

            Console.Write("Entrez le message à envoyer : ");
            string? message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message)) return;

            using var client = new HttpClient();
            var payload = new { content = message };
            var json = JsonSerializer.Serialize(payload);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, data);
            
            if (response.IsSuccessStatusCode) Console.WriteLine("[+] Message envoyé !");
            else Console.WriteLine($"[-] Erreur: {response.StatusCode}");
        }

        public static async Task HandleGuildClonerMenu()
        {
            var subOptions = new List<string> { "Copier un serveur (.json)", "Coller un serveur (depuis .json)", "Retour" };
            int selectedIndex = UIHelper.SingleChoice(subOptions);
            Console.Clear();
            if (selectedIndex == 0) await HandleGuildCloner();
            else if (selectedIndex == 1) await HandleGuildPaster();
        }

        private static async Task HandleGuildCloner()
        {
            Console.Write("Entrez le Token du BOT : ");
            string? token = Console.ReadLine()?.Trim();
            Console.Write("Entrez l'ID du serveur à copier : ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong guildId)) return;

            var config = new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds };
            using var client = new DiscordSocketClient(config);
            var tcs = new TaskCompletionSource<bool>();
            client.Ready += () => { tcs.SetResult(true); return Task.CompletedTask; };

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await tcs.Task;

            var guild = client.GetGuild(guildId);
            if (guild == null) { await client.StopAsync(); return; }

            var serverData = new GuildData
            {
                Name = guild.Name,
                Roles = guild.Roles.Select(r => new RoleData { Name = r.Name, Color = r.Color.RawValue, Permissions = r.Permissions.RawValue }).ToList(),
                Categories = guild.CategoryChannels.Select(c => new CategoryData { 
                    Name = c.Name, 
                    Channels = c.Channels.Select(ch => new ChannelData { Name = ch.Name, Type = ch is ITextChannel ? "Text" : "Voice" }).ToList() 
                }).ToList()
            };

            File.WriteAllText($"guild_{guildId}.json", JsonSerializer.Serialize(serverData, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("[+] Serveur copié !");
            await client.StopAsync();
        }

        private static async Task HandleGuildPaster()
        {
            // Implémentation simplifiée pour le moment
            Console.WriteLine("[*] Option de collage en cours de maintenance...");
        }
    }
}
