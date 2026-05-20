using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class SherlockService
    {
        private static readonly Dictionary<string, string> SocialPlatforms = new()
        {
            { "GitHub", "https://github.com/{0}" },
            { "Twitter/X", "https://twitter.com/{0}" },
            { "Instagram", "https://www.instagram.com/{0}/" },
            { "TikTok", "https://www.tiktok.com/@{0}" },
            { "Reddit", "https://www.reddit.com/user/{0}" },
            { "Pinterest", "https://www.pinterest.com/{0}/" },
            { "Snapchat", "https://www.snapchat.com/add/{0}" },
            { "Telegram", "https://t.me/{0}" },
            { "Steam", "https://steamcommunity.com/id/{0}" },
            { "Twitch", "https://www.twitch.tv/{0}" },
            { "Roblox", "https://www.roblox.com/user.aspx?username={0}" },
            { "SoundCloud", "https://soundcloud.com/{0}" },
            { "Spotify", "https://open.spotify.com/user/{0}" },
            { "YouTube", "https://www.youtube.com/@{0}" },
            { "Medium", "https://medium.com/@{0}" },
            { "About.me", "https://about.me/{0}" },
            { "Chess.com", "https://www.chess.com/member/{0}" },
            { "Duolingo", "https://www.duolingo.com/profile/{0}" },
            { "Fiverr", "https://www.fiverr.com/{0}" },
            { "Letterboxd", "https://letterboxd.com/{0}/" }
        };

        public static async Task HandleSherlockMenu()
        {
            UIHelper.PrintSectionHeader("OSINT LOOKUP");
            string username = UIHelper.Prompt("Username à rechercher") ?? "";
            if (string.IsNullOrEmpty(username)) return;

            UIHelper.PrintInfo($"Recherche de '{username}' sur {SocialPlatforms.Count} plateformes...");
            Console.WriteLine();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(10);

            var tasks = new List<Task>();
            int foundCount = 0;

            foreach (var platform in SocialPlatforms)
            {
                tasks.Add(Task.Run(async () =>
                {
                    string url = string.Format(platform.Value, username);
                    try
                    {
                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            UIHelper.PrintSuccess($"{platform.Key.PadRight(12)}: {url}");
                            foundCount++;
                        }
                    }
                    catch { /* Ignore timeouts/connection errors for speed */ }
                }));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine();
            UIHelper.PrintInfo($"Recherche terminée. {foundCount} profil(s) trouvé(s).");
            UIHelper.PressAnyKey();
        }
    }
}
