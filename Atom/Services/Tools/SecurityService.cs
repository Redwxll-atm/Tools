using System;
using System.IO;
using System.Linq;

namespace Atom.Services.Tools
{
    public static class SecurityService
    {
        public static void HandleRemoveDiscordInjection()
        {
            Console.WriteLine("=== REMOVE DISCORD INJECTION ===");
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string[] discordPaths = {
                Path.Combine(localAppData, "Discord"),
                Path.Combine(localAppData, "DiscordCanary"),
                Path.Combine(localAppData, "DiscordPTB"),
                Path.Combine(localAppData, "DiscordDevelopment")
            };

            bool found = false;
            foreach (var path in discordPaths)
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"[*] Analyse de {Path.GetFileName(path)}...");
                    var coreDirs = Directory.GetDirectories(path, "app-*");
                    foreach (var coreDir in coreDirs)
                    {
                        string modulesPath = Path.Combine(coreDir, "modules");
                        if (Directory.Exists(modulesPath))
                        {
                            var discordCoreDirs = Directory.GetDirectories(modulesPath, "discord_desktop_core-*");
                            foreach (var ddcDir in discordCoreDirs)
                            {
                                string indexFile = Path.Combine(ddcDir, "discord_desktop_core", "index.js");
                                if (File.Exists(indexFile))
                                {
                                    string content = File.ReadAllText(indexFile);
                                    if (content != "module.exports = require('./core.asar');")
                                    {
                                        Console.WriteLine($"[!] Injection détectée dans {indexFile}!");
                                        File.WriteAllText(indexFile, "module.exports = require('./core.asar');");
                                        Console.WriteLine("[+] Nettoyage effectué.");
                                        found = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!found) Console.WriteLine("[+] Aucune injection détectée.");
        }
    }
}
