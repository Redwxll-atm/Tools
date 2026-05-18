using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class DiscordTokenStatusService
    {
        private static bool _isRunning = false;

        public static async Task HandleTokenStatusMenu()
        {
            UIHelper.PrintSectionHeader("Token Custom Status");

            if (_isRunning)
            {
                UIHelper.PrintWarning("Une session est déjà active.");
                var subOptions = new List<string> { "Arrêter la session", "Retour" };
                int subChoice = UIHelper.SingleChoice(subOptions);

                if (subChoice == 0)
                {
                    _isRunning = false;
                    UIHelper.PrintSuccess("Session arrêtée.");
                }
                return;
            }

            string token = UIHelper.Prompt("Token du compte") ?? "";
            if (string.IsNullOrEmpty(token)) return;

            string text = UIHelper.Prompt("Texte du statut personnalisé") ?? "ATOM Multi-Tool";
            
            UIHelper.PrintInfo("Choisissez l'état de présence :");
            var presenceOptions = new List<string> { "Online", "Idle", "DND", "Invisible", "Retour" };
            int pChoice = UIHelper.SingleChoice(presenceOptions);
            if (pChoice == 4) return;

            string status = pChoice switch
            {
                0 => "online",
                1 => "idle",
                2 => "dnd",
                3 => "invisible",
                _ => "online"
            };

            _isRunning = true;
            await UpdateStatus(token, text, status);
        }

        private static async Task UpdateStatus(string token, string text, string status)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");

            try
            {
                var payload = new 
                { 
                    custom_status = new { text = text },
                    status = status
                };
                
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PatchAsync("https://discord.com/api/v10/users/@me/settings", content);

                if (response.IsSuccessStatusCode)
                {
                    UIHelper.PrintSuccess($"Statut '{text}' ({status}) activé !");
                }
                else
                {
                    UIHelper.PrintError($"Erreur API: {response.StatusCode}");
                    _isRunning = false;
                }
            }
            catch (Exception ex)
            {
                UIHelper.PrintError($"Erreur : {ex.Message}");
                _isRunning = false;
            }
        }

        public static Task StopStatus()
        {
            _isRunning = false;
            UIHelper.PrintSuccess("Session Token arrêtée.");
            return Task.CompletedTask;
        }
    }
}
