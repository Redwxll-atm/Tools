using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Atom.Services
{
    public static class UpdateService
    {
        public const string CurrentVersion = "1.0.0";
        private const string RepoOwner = "Redwxll-atm";
        private const string RepoName = "Tools";

        public static async Task CheckForUpdates()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Atom-Tool", "1.0"));
                    string url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";
                    
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode) return;

                    var content = await response.Content.ReadAsStringAsync();
                    // Basic JSON parsing to avoid heavy dependencies
                    string latestVersion = ExtractTag(content);

                    if (!string.IsNullOrEmpty(latestVersion) && latestVersion != CurrentVersion)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n[!] Une nouvelle version est disponible : {latestVersion} (Actuelle: {CurrentVersion})");
                        Console.WriteLine("[!] Mise à jour automatique en cours...");
                        Console.ResetColor();
                        
                        await PerformUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur lors de la vérification des mises à jour : {ex.Message}");
            }
        }

        private static string ExtractTag(string json)
        {
            int tagIndex = json.IndexOf("\"tag_name\":");
            if (tagIndex == -1) return null;
            
            int start = json.IndexOf("\"", tagIndex + 11) + 1;
            int end = json.IndexOf("\"", start);
            return json.Substring(start, end - start);
        }

        private static async Task PerformUpdate()
        {
            // Note: In a real environment, this would download the new binary and restart.
            // For this specific request, we simulate the 'local update' by pulling git if available
            // or informing the user to pull the latest changes.
            
            Console.WriteLine("[*] Récupération des fichiers depuis GitHub...");
            
            try
            {
                // Simple attempt to run git pull if we are in a repo
                var process = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "pull",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using (var p = Process.Start(process))
                {
                    await p.WaitForExitAsync();
                    if (p.ExitCode == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[+] Mise à jour réussie ! Veuillez redémarrer l'application.");
                        Console.ResetColor();
                        Thread.Sleep(2000);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("[!] Échec du 'git pull'. Veuillez télécharger la dernière release manuellement.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("[!] Git non trouvé. Veuillez télécharger la dernière release sur GitHub.");
            }
        }
    }
}
