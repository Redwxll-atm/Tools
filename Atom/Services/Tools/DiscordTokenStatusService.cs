using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class DiscordTokenStatusService
    {
        private static DiscordSocketClient? _client;

        public static async Task HandleTokenStatusMenu()
        {
            UIHelper.PrintSectionHeader("Token Custom Status");

            if (_client != null)
            {
                UIHelper.PrintWarning("Une session est déjà active.");
                var subOptions = new List<string> { "Arrêter la session", "Retour" };
                int subChoice = UIHelper.SingleChoice(subOptions);

                if (subChoice == 0)
                {
                    await StopStatus();
                }
                return;
            }

            string token = UIHelper.Prompt("Token du compte") ?? "";
            if (string.IsNullOrEmpty(token)) return;

            UIHelper.PrintInfo("Choisissez le type de statut :");
            var types = new List<string> { "Streaming (Twitch)", "Playing", "Watching", "Listening", "Retour" };
            int typeChoice = UIHelper.SingleChoice(types);

            if (typeChoice == 4) return;

            string text = UIHelper.Prompt("Texte du statut") ?? "ATOM Multi-Tool";
            string? url = null;

            if (typeChoice == 0)
            {
                url = UIHelper.Prompt("Lien Twitch (ex: https://twitch.tv/username)") ?? "https://twitch.tv/discord";
            }

            ActivityType activityType = typeChoice switch
            {
                0 => ActivityType.Streaming,
                1 => ActivityType.Playing,
                2 => ActivityType.Watching,
                3 => ActivityType.Listening,
                _ => ActivityType.Playing
            };

            await StartStatus(token, text, activityType, url);
        }

        private static async Task StartStatus(string token, string text, ActivityType type, string? url)
        {
            try
            {
                var config = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.None,
                    LogLevel = LogSeverity.Error
                };

                _client = new DiscordSocketClient(config);

                _client.Ready += async () =>
                {
                    UIHelper.PrintSuccess($"Connecté sur le compte : {_client.CurrentUser.Username}");
                    await _client.SetGameAsync(text, url, type);
                    UIHelper.PrintSuccess($"Statut '{text}' activé !");
                };

                // Note: Discord.Net does not officially support user tokens (self-bots).
                // This might work with some user tokens but is against Discord TOS.
                await _client.LoginAsync(TokenType.Bot, token); 
                await _client.StartAsync();

                UIHelper.PrintInfo("Note: Le statut restera actif tant qu'ATOM est ouvert.");
            }
            catch (Exception ex)
            {
                UIHelper.PrintError($"Erreur : {ex.Message}");
                _client = null;
            }
        }

        public static async Task StopStatus()
        {
            if (_client != null)
            {
                await _client.StopAsync();
                await _client.LogoutAsync();
                _client.Dispose();
                _client = null;
                UIHelper.PrintSuccess("Session Token arrêtée.");
            }
            else
            {
                UIHelper.PrintInfo("Aucune session active.");
            }
        }
    }
}
