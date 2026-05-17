using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Atom.Services;
using Atom.Services.Tools;
using Atom.Utils;

namespace Atom
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = $"ATOM | v{UpdateService.CurrentVersion}";
            await UpdateService.CheckForUpdates();

            var mainOptions = new List<string> 
            { 
                "Discord Webhook Tools", 
                "Token Tools", 
                "Proxy Tools", 
                "System & Security Tools", 
                "Network & Utility Tools",
                "Quitter" 
            };
            
            while (true)
            {
                UIHelper.DisplayHeader();
                
                int selectedIndex = UIHelper.SingleChoice(mainOptions);

                if (mainOptions[selectedIndex] == "Quitter")
                {
                    Console.Clear();
                    UIHelper.DisplayHeader();
                    Console.ForegroundColor = ConsoleColor.Red;
                    string goodbye = "Merci d'avoir utilisé ATOM. À bientôt !";
                    int left = Math.Max(0, (Console.WindowWidth - goodbye.Length) / 2);
                    Console.SetCursorPosition(left, Console.CursorTop);
                    Console.WriteLine(goodbye);
                    Console.ResetColor();
                    Thread.Sleep(2000);
                    break;
                }

                UIHelper.TransitionEffect();
                switch (selectedIndex)
                {
                    case 0:
                        await HandleDiscordMenu();
                        break;
                    case 1:
                        await HandleTokenMenu();
                        break;
                    case 2:
                        await HandleProxyMenu();
                        break;
                    case 3:
                        await HandleSystemMenu();
                        break;
                    case 4:
                        await HandleUtilityMenu();
                        break;
                }
            }
        }

        static async Task HandleDiscordMenu()
        {
            UIHelper.DisplayHeader();
            var options = new List<string> 
            { 
                "Webhook Tools (Check/Delete/Send)", 
                "Guild Cloner", 
                "Token Profile Editor",
                "Avatar Scraper",
                "Discord Rich Presence (App)",
                "Token Custom Status (Account)",
                "Retour" 
            };
            int choice = UIHelper.SingleChoice(options);
            UIHelper.TransitionEffect();
            switch (choice)
            {
                case 0: await WebhookService.HandleWebhookMenu(); break;
                case 1: await DiscordService.HandleGuildClonerMenu(); break;
                case 2: await DiscordService.HandleTokenEditorMenu(); break;
                case 3: await DiscordService.HandleAvatarScraper(); break;
                case 4: DiscordRpcService.HandleRpcMenu(); break;
                case 5: await DiscordTokenStatusService.HandleTokenStatusMenu(); break;
            }
            if (choice != 6 && choice != 4 && choice != 5) { Console.WriteLine("\nAppuyez sur une touche..."); Console.ReadKey(); }
            else if (choice == 4 || choice == 5) { UIHelper.PressAnyKey(); }
        }

        static async Task HandleTokenMenu()
        {
            UIHelper.DisplayHeader();
            var options = new List<string> 
            { 
                "Token Checker/Info", 
                "Token Formatter", 
                "Token Sorter", 
                "Remove Duplicates", 
                "Token Status Rotator",
                "Nitro Gift Checker",
                "Payment Method Checker",
                "Guild Management (List/Leave)",
                "Retour" 
            };
            int choice = UIHelper.SingleChoice(options);
            UIHelper.TransitionEffect();
            switch (choice)
            {
                case 0: await TokenService.HandleTokenInfo(); break;
                case 1: TokenService.HandleTokenFormatter(); break;
                case 2: TokenService.HandleTokenSorter(); break;
                case 3: TokenService.HandleRemoveDuplicates(); break;
                case 4: await TokenService.HandleStatusRotator(); break;
                case 5: await TokenService.HandleNitroChecker(); break;
                case 6: await TokenAdvancedService.HandlePaymentChecker(); break;
                case 7: await TokenAdvancedService.HandleGuildTools(); break;
            }
            if (choice != 8) { Console.WriteLine("\nAppuyez sur une touche..."); Console.ReadKey(); }
        }

        static async Task HandleProxyMenu()
        {
            UIHelper.DisplayHeader();
            var options = new List<string> { "Proxy Scraper", "Proxy Checker", "Retour" };
            int choice = UIHelper.SingleChoice(options);
            UIHelper.TransitionEffect();
            if (choice == 0) await ProxyService.HandleProxyScraper();
            else if (choice == 1) await ProxyService.HandleProxyChecker();
            if (choice != 2) { Console.WriteLine("\nAppuyez sur une touche..."); Console.ReadKey(); }
        }

        static async Task HandleSystemMenu()
        {
            UIHelper.DisplayHeader();
            var options = new List<string> 
            { 
                "HWID Grabber",
                "MAC Address Spoofer", 
                "Serial Checker", 
                "Serial Changer (HWID)", 
                "DLL Injector",
                "Remove Discord Injection", 
                "PC Cleaner (Temp/CrashDumps)",
                "Retour" 
            };
            int choice = UIHelper.SingleChoice(options);
            UIHelper.TransitionEffect();
            switch (choice)
            {
                case 0: SystemService.HandleHWIDTool(); break;
                case 1: MacSpooferService.HandleMacSpoofer(); break;
                case 2: SystemService.HandleSerialChecker(); break;
                case 3: SystemService.HandleSerialChanger(); break;
                case 4: InjectorService.HandleInjectorMenu(); break;
                case 5: SecurityService.HandleRemoveDiscordInjection(); break;
                case 6: CleanerService.HandleCleanerMenu(); break;
            }
            if (choice != 7) { Console.WriteLine("\nAppuyez sur une touche..."); Console.ReadKey(); }
        }

        static async Task HandleUtilityMenu()
        {
            UIHelper.DisplayHeader();
            var options = new List<string> 
            { 
                "IP Lookup", 
                "Faker Tools (Identity/Card/Token)",
                "QR Code Generator",
                "YouTube Converter", 
                "Retour" 
            };
            int choice = UIHelper.SingleChoice(options);
            UIHelper.TransitionEffect();
            switch (choice)
            {
                case 0: await NetworkService.HandleIpLookup(); break;
                case 1: FakerService.HandleFakerMenu(); break;
                case 2: QrCodeService.HandleQrCodeGenerator(); break;
                case 3: Console.WriteLine("[*] YouTube Converter: Option bientôt disponible..."); break;
            }
            if (choice != 4) { Console.WriteLine("\nAppuyez sur une touche..."); Console.ReadKey(); }
        }
    }
}
