using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Atom.Services.Tools
{
    public static class ProxyService
    {
        public static async Task HandleProxyScraper()
        {
            Console.WriteLine("=== PROXY SCRAPER ===");
            var sources = new List<string>
            {
                "https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=10000&country=all&ssl=all&anonymity=all",
                "https://www.proxy-list.download/api/v1/get?type=http",
                "https://www.proxyscan.io/download?type=http"
            };

            using var client = new HttpClient();
            var allProxies = new HashSet<string>();

            foreach (var url in sources)
            {
                try
                {
                    Console.WriteLine($"[*] Scraping {new Uri(url).Host}...");
                    var content = await client.GetStringAsync(url);
                    var proxies = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in proxies) allProxies.Add(p.Trim());
                }
                catch { }
            }

            File.WriteAllLines("proxies.txt", allProxies);
            Console.WriteLine($"[+] {allProxies.Count} proxies récupérés dans proxies.txt");
        }

        public static async Task HandleProxyChecker()
        {
            Console.WriteLine("=== PROXY CHECKER ===");
            Console.Write("Chemin du fichier proxies: ");
            string? path = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            var proxies = File.ReadAllLines(path);
            var validProxies = new List<string>();

            Console.WriteLine($"[*] Test de {proxies.Length} proxies...");

            foreach (var proxyStr in proxies)
            {
                if (await CheckProxy(proxyStr))
                {
                    Console.WriteLine($"[+] {proxyStr} est VALIDE");
                    validProxies.Add(proxyStr);
                }
            }

            File.WriteAllLines("valid_proxies.txt", validProxies);
            Console.WriteLine($"[+] Terminé. {validProxies.Count} proxies valides enregistrés.");
        }

        private static async Task<bool> CheckProxy(string proxyStr)
        {
            try
            {
                var proxy = new WebProxy(proxyStr);
                var handler = new HttpClientHandler { Proxy = proxy, UseProxy = true };
                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetAsync("http://www.google.com");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
