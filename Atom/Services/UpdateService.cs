using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Atom.Utils;

namespace Atom.Services
{
    public static class UpdateService
    {
        public const string CurrentVersion = "1.0.3";
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
                    string latestVersion = ExtractTag(content);
                    string downloadUrl = ExtractDownloadUrl(content);

                    if (!string.IsNullOrEmpty(latestVersion) && latestVersion != CurrentVersion && !string.IsNullOrEmpty(downloadUrl))
                    {
                        UIHelper.DisplayHeader();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n[!] Une nouvelle version est disponible : {latestVersion} (Actuelle: {CurrentVersion})");
                        Console.WriteLine("[!] Mise à jour automatique en cours...");
                        Console.ResetColor();
                        
                        await PerformUpdate(latestVersion, downloadUrl);
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

        private static string ExtractDownloadUrl(string json)
        {
            int assetIndex = json.IndexOf("\"name\": \"Atom.exe\"");
            if (assetIndex == -1) assetIndex = json.IndexOf(".exe\"");
            
            if (assetIndex == -1) return null;

            int urlIndex = json.IndexOf("\"browser_download_url\":", assetIndex);
            if (urlIndex == -1) return null;

            int start = json.IndexOf("\"", urlIndex + 23) + 1;
            int end = json.IndexOf("\"", start);
            return json.Substring(start, end - start);
        }

        private static async Task PerformUpdate(string latestVersion, string downloadUrl)
        {
            Console.WriteLine("[*] Téléchargement de la nouvelle version...");
            
            try
            {
                string currentExePath = Process.GetCurrentProcess().MainModule.FileName;
                string tempExePath = currentExePath + ".new";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Atom-Tool", "1.0"));
                    var response = await client.GetAsync(downloadUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[!] Échec du téléchargement (Code: {response.StatusCode})");
                        return;
                    }

                    using (var fs = new FileStream(tempExePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] Téléchargement terminé. Redémarrage pour appliquer la mise à jour...");
                Console.ResetColor();

                string batchPath = Path.Combine(Path.GetTempPath(), "atom_update.bat");
                string batchContent = $@"
@echo off
timeout /t 2 /nobreak > nul
del ""{currentExePath}""
move ""{tempExePath}"" ""{currentExePath}""
start """" ""{currentExePath}""
del ""%~f0""
";
                File.WriteAllText(batchPath, batchContent);

                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{batchPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur lors de la mise à jour : {ex.Message}");
                Console.WriteLine("[!] Veuillez télécharger la dernière version manuellement sur GitHub.");
                Thread.Sleep(5000);
            }
        }
    }
}
