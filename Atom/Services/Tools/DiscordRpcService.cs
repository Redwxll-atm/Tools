using System;
using DiscordRPC;
using DiscordRPC.Logging;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class DiscordRpcService
    {
        private static DiscordRpcClient? _client;

        public static void HandleRpcMenu()
        {
            UIHelper.PrintSectionHeader("Discord Rich Presence");

            if (_client != null)
            {
                UIHelper.PrintWarning("Une présence est déjà active.");
                var subOptions = new List<string> { "Modifier la présence", "Arrêter la présence", "Retour" };
                int subChoice = UIHelper.SingleChoice(subOptions);

                if (subChoice == 1)
                {
                    StopRpc();
                    return;
                }
                else if (subChoice == 2)
                {
                    return;
                }
            }

            string clientId = UIHelper.Prompt("Client ID (depuis Discord Developer Portal)") ?? "";
            if (string.IsNullOrEmpty(clientId)) return;

            string details = UIHelper.Prompt("Détails (Ligne 1)") ?? "Utilise ATOM";
            string state = UIHelper.Prompt("État (Ligne 2)") ?? "The Ultimate Multi-Tool";
            string largeKey = UIHelper.Prompt("Large Image Key (optionnel)") ?? "";
            string largeText = UIHelper.Prompt("Large Image Text (optionnel)") ?? "";

            StartRpc(clientId, details, state, largeKey, largeText);
        }

        private static void StartRpc(string clientId, string details, string state, string largeKey, string largeText)
        {
            try
            {
                if (_client != null) _client.Dispose();

                _client = new DiscordRpcClient(clientId)
                {
                    Logger = new ConsoleLogger() { Level = LogLevel.Warning }
                };

                _client.OnReady += (sender, e) =>
                {
                    UIHelper.PrintSuccess($"Connecté à Discord pour l'utilisateur : {e.User.Username}");
                };

                _client.Initialize();

                _client.SetPresence(new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = string.IsNullOrEmpty(largeKey) ? null : largeKey,
                        LargeImageText = string.IsNullOrEmpty(largeText) ? null : largeText
                    },
                    Timestamps = Timestamps.Now
                });

                UIHelper.PrintSuccess("Rich Presence activée !");
                UIHelper.PrintInfo("Note: La présence restera active tant qu'ATOM est ouvert.");
            }
            catch (Exception ex)
            {
                UIHelper.PrintError($"Erreur RPC : {ex.Message}");
            }
        }

        public static void StopRpc()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
                UIHelper.PrintSuccess("Rich Presence arrêtée.");
            }
            else
            {
                UIHelper.PrintInfo("Aucune présence n'est active.");
            }
        }
    }
}
