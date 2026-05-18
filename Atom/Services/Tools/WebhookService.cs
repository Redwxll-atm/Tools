using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class WebhookService
    {
        public static async Task HandleWebhookMenu()
        {
            var options = new List<string> 
            { 
                "CHECK WEBHOOK",
                "WEBHOOK INFO",
                "DELETE WEBHOOK",
                "SPAM WEBHOOK",
                "CREATE WEBHOOKS",
                "CREATE + SPAM WEBHOOKS",
                "Retour" 
            };
            
            int choice = UIHelper.SingleChoice(options);
            Console.Clear();

            switch (choice)
            {
                case 0: await CheckWebhook(false); break;
                case 1: await CheckWebhook(true); break;
                case 2: await DeleteWebhook(); break;
                case 3: await SpamWebhook(); break;
                case 4: await CreateWebhooks(); break;
                case 5: await CreateAndSpamWebhooks(); break;
            }
        }

        private static async Task CheckWebhook(bool showInfo)
        {
            UIHelper.PrintSectionHeader(showInfo ? "WEBHOOK INFO" : "CHECK WEBHOOK");
            string url = UIHelper.Prompt("URL du Webhook") ?? "";
            if (string.IsNullOrEmpty(url)) return;

            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    UIHelper.PrintSuccess("Webhook Valide !");
                    if (showInfo)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(json);
                        var root = doc.RootElement;

                        Console.WriteLine();
                        UIHelper.PrintField("Nom", root.GetProperty("name").GetString() ?? "N/A");
                        UIHelper.PrintField("ID", root.GetProperty("id").GetString() ?? "N/A");
                        UIHelper.PrintField("Channel ID", root.GetProperty("channel_id").GetString() ?? "N/A");
                        UIHelper.PrintField("Guild ID", root.GetProperty("guild_id").GetString() ?? "N/A");
                        if (root.TryGetProperty("token", out var token))
                            UIHelper.PrintField("Token", token.GetString() ?? "N/A");
                    }
                }
                else
                {
                    UIHelper.PrintError($"Webhook Invalide (Code: {response.StatusCode})");
                }
            }
            catch (Exception ex) { UIHelper.PrintError($"Erreur: {ex.Message}"); }
            UIHelper.PressAnyKey();
        }

        private static async Task DeleteWebhook()
        {
            UIHelper.PrintSectionHeader("DELETE WEBHOOK");
            string url = UIHelper.Prompt("URL du Webhook à supprimer") ?? "";
            if (string.IsNullOrEmpty(url)) return;

            using var client = new HttpClient();
            try
            {
                var response = await client.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                    UIHelper.PrintSuccess("Webhook supprimé avec succès.");
                else
                    UIHelper.PrintError($"Échec de la suppression (Code: {response.StatusCode})");
            }
            catch (Exception ex) { UIHelper.PrintError($"Erreur: {ex.Message}"); }
            UIHelper.PressAnyKey();
        }

        private static async Task SpamWebhook()
        {
            UIHelper.PrintSectionHeader("SPAM WEBHOOK");
            string url = UIHelper.Prompt("URL du Webhook") ?? "";
            string message = UIHelper.Prompt("Message à spam") ?? "ATOM Multi-Tool Spam";
            if (!int.TryParse(UIHelper.Prompt("Nombre de messages"), out int count)) count = 10;
            if (!int.TryParse(UIHelper.Prompt("Délai (ms)"), out int delay)) delay = 1000;

            if (string.IsNullOrEmpty(url)) return;

            UIHelper.PrintInfo($"Démarrage du spam ({count} messages)...");
            using var client = new HttpClient();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var payload = new { content = message };
                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                        UIHelper.PrintSuccess($"[{i+1}/{count}] Message envoyé !");
                    else
                        UIHelper.PrintError($"[{i+1}/{count}] Erreur: {response.StatusCode}");
                }
                catch (Exception ex) { UIHelper.PrintError($"[!] Erreur: {ex.Message}"); }
                await Task.Delay(delay);
            }
            UIHelper.PrintInfo("Spam terminé.");
            UIHelper.PressAnyKey();
        }

        private static async Task CreateWebhooks()
        {
            UIHelper.PrintSectionHeader("CREATE WEBHOOKS");
            string token = UIHelper.Prompt("Token du compte (Bot ou User)") ?? "";
            string channelId = UIHelper.Prompt("ID du Channel") ?? "";
            string name = UIHelper.Prompt("Nom des webhooks") ?? "ATOM Webhook";
            if (!int.TryParse(UIHelper.Prompt("Nombre de webhooks à créer"), out int count)) count = 1;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(channelId)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            for (int i = 0; i < count; i++)
            {
                try
                {
                    var payload = new { name = $"{name} #{i+1}" };
                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://discord.com/api/v10/channels/{channelId}/webhooks", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(json);
                        string url = $"https://discord.com/api/webhooks/{doc.RootElement.GetProperty("id").GetString()}/{doc.RootElement.GetProperty("token").GetString()}";
                        UIHelper.PrintSuccess($"Webhook créé: {url}");
                    }
                    else UIHelper.PrintError($"Erreur ({response.StatusCode}): {await response.Content.ReadAsStringAsync()}");
                }
                catch (Exception ex) { UIHelper.PrintError($"Erreur: {ex.Message}"); }
                await Task.Delay(500);
            }
            UIHelper.PressAnyKey();
        }

        private static async Task CreateAndSpamWebhooks()
        {
            UIHelper.PrintSectionHeader("CREATE + SPAM WEBHOOKS");
            string token = UIHelper.Prompt("Token du compte") ?? "";
            string channelId = UIHelper.Prompt("ID du Channel") ?? "";
            string message = UIHelper.Prompt("Message à spam") ?? "ATOM Multi-Tool Spam";
            if (!int.TryParse(UIHelper.Prompt("Nombre de webhooks"), out int whCount)) whCount = 3;
            if (!int.TryParse(UIHelper.Prompt("Messages par webhook"), out int msgCount)) msgCount = 5;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(channelId)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            var urls = new List<string>();
            UIHelper.PrintInfo("Création des webhooks...");
            for (int i = 0; i < whCount; i++)
            {
                var payload = new { name = $"ATOM Destroyer #{i+1}" };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://discord.com/api/v10/channels/{channelId}/webhooks", content);
                if (response.IsSuccessStatusCode)
                {
                    var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    urls.Add($"https://discord.com/api/webhooks/{doc.RootElement.GetProperty("id").GetString()}/{doc.RootElement.GetProperty("token").GetString()}");
                }
            }

            UIHelper.PrintInfo($"Démarrage du spam massif ({urls.Count} webhooks)...");
            var tasks = new List<Task>();
            foreach (var url in urls)
            {
                tasks.Add(Task.Run(async () => {
                    using var whClient = new HttpClient();
                    for (int j = 0; j < msgCount; j++)
                    {
                        var payload = new { content = message };
                        await whClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
                        await Task.Delay(500);
                    }
                }));
            }
            await Task.WhenAll(tasks);
            UIHelper.PrintSuccess("Spam massif terminé.");
            UIHelper.PressAnyKey();
        }
    }
}
