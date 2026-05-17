using System;
using System.Collections.Generic;
using System.Threading;

namespace Atom.Utils
{
    public static class UIHelper
    {
        // ─── Palette ────────────────────────────────────────────────────────────────
        private static readonly ConsoleColor AccentPrimary = ConsoleColor.Red;
        private static readonly ConsoleColor AccentSecondary = ConsoleColor.DarkRed;
        private static readonly ConsoleColor TextBright = ConsoleColor.White;
        private static readonly ConsoleColor TextDim = ConsoleColor.DarkGray;
        private static readonly ConsoleColor TextMuted = ConsoleColor.Gray;
        private static readonly ConsoleColor Success = ConsoleColor.Green;
        private static readonly ConsoleColor Warning = ConsoleColor.Yellow;
        private static readonly ConsoleColor Error = ConsoleColor.Red;

        // ─── ASCII Art ───────────────────────────────────────────────────────────────
        // Double espace uniforme entre chaque lettre — A aligné pixel-perfect
        private static readonly string[] AsciiLogo =
        {
            @"  ░█████╗░  ████████╗  ░█████╗░  ███╗░░░███╗",
            @"  ██╔══██╗  ╚══██╔══╝  ██╔══██╗  ████╗░████║",
            @"  ███████║  ░░░██║░░░  ██║░░██║  ██╔████╔██║",
            @"  ██╔══██║  ░░░██║░░░  ██║░░██║  ██║╚██╔╝██║",
            @"  ██║░░██║  ░░░██║░░░  ╚█████╔╝  ██║░╚═╝░██║",
            @"  ╚═╝░░╚═╝  ░░╚═╝░░░  ░╚════╝░  ╚═╝░░░░░╚═╝",
        };

        // Dégradé dark → bright → dark
        private static readonly ConsoleColor[] AsciiLogoColors =
        {
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.Red,
            ConsoleColor.Red,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
        };

        // ─── Print helpers ───────────────────────────────────────────────────────────

        public static void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void PrintSuccess(string msg)
        {
            Console.ForegroundColor = Success;
            Console.Write("  [+] ");
            Console.ForegroundColor = TextBright;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintError(string msg)
        {
            Console.ForegroundColor = Error;
            Console.Write("  [-] ");
            Console.ForegroundColor = TextMuted;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintWarning(string msg)
        {
            Console.ForegroundColor = Warning;
            Console.Write("  [!] ");
            Console.ForegroundColor = TextMuted;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintInfo(string msg)
        {
            Console.ForegroundColor = AccentSecondary;
            Console.Write("  [*] ");
            Console.ForegroundColor = TextMuted;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintField(string label, string value, int labelWidth = 10)
        {
            Console.ForegroundColor = TextDim;
            Console.Write($"  {label.PadRight(labelWidth)} ");
            Console.ForegroundColor = AccentSecondary;
            Console.Write(": ");
            Console.ForegroundColor = TextBright;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        public static string? Prompt(string question)
        {
            Console.ForegroundColor = AccentSecondary;
            Console.Write("\n  ❯ ");
            Console.ForegroundColor = TextBright;
            Console.Write($"{question}: ");
            Console.ForegroundColor = TextMuted;
            string? input = Console.ReadLine();
            Console.ResetColor();
            return input?.Trim();
        }

        public static void PressAnyKey()
        {
            Console.WriteLine();
            Console.ForegroundColor = TextDim;
            CenterWrite("Appuyez sur une touche pour continuer…");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        // ─── Section header ──────────────────────────────────────────────────────────

        public static void PrintSectionHeader(string title)
        {
            Console.WriteLine();
            int width = Math.Min(Console.WindowWidth - 4, 50);
            string inner = $"  {title.ToUpper()}  ".PadRight(width);

            Console.ForegroundColor = AccentSecondary;
            Console.WriteLine($"  ╔{new string('═', width)}╗");
            Console.ForegroundColor = AccentPrimary;
            Console.Write("  ║");
            Console.ForegroundColor = TextBright;
            Console.Write(inner);
            Console.ForegroundColor = AccentPrimary;
            Console.WriteLine("║");
            Console.ForegroundColor = AccentSecondary;
            Console.WriteLine($"  ╚{new string('═', width)}╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        // ─── DisplayHeader ───────────────────────────────────────────────────────────

        public static void DisplayHeader(string version = "")
        {
            Console.Clear();

            if (OperatingSystem.IsWindows())
                try { Console.Title = "ATOM — The Ultimate Multi-Tool"; } catch { }

            int w = Console.WindowWidth;

            // ── Version Tag (Top-Left) ──────────────────────────────────────
            if (!string.IsNullOrEmpty(version))
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = AccentSecondary;
                Console.Write(" ▓");
                Console.ForegroundColor = TextBright;
                Console.Write($" v{version} ");
                Console.ForegroundColor = AccentSecondary;
                Console.WriteLine("▓");
            }

            int logoWidth = 0;
            foreach (var line in AsciiLogo)
                if (line.Length > logoWidth) logoWidth = line.Length;

            int borderW = Math.Max(logoWidth + 4, Math.Min(w - 2, 60));
            string border = new string('▓', borderW);

            // ── Top border ──────────────────────────────────────────────────
            Console.ForegroundColor = AccentSecondary;
            CenterWrite(border);
            Console.ResetColor();
            Console.WriteLine();

            // ── Logo ────────────────────────────────────────────────────────
            for (int i = 0; i < AsciiLogo.Length; i++)
            {
                Console.ForegroundColor = AsciiLogoColors[i];
                CenterWrite(AsciiLogo[i]);
            }

            // ── Subtitle ────────────────────────────────────────────────────
            Console.WriteLine();
            string core = string.IsNullOrEmpty(version)
                ? " THE·ULTIMATE·MULTI·TOOL "
                : $" THE·ULTIMATE·MULTI·TOOL  [v{version}] ";

            int padEach = Math.Max(2, (borderW - core.Length) / 2);
            string sub = new string('░', padEach) + core + new string('░', padEach);
            if (sub.Length < borderW) sub += "░";
            if (sub.Length > borderW) sub = sub[..borderW];

            Console.ForegroundColor = TextDim;
            CenterWrite(sub);
            Console.ResetColor();

            // ── Bottom border ───────────────────────────────────────────────
            Console.WriteLine();
            Console.ForegroundColor = AccentSecondary;
            CenterWrite(border);
            Console.ResetColor();

            // ── Statusbar ───────────────────────────────────────────────────
            Console.WriteLine();
            PrintStatusBar(version, w);
            Console.WriteLine();
        }

        // ─── SingleChoice ────────────────────────────────────────────────────────────

        public static int SingleChoice(List<string> options, string? title = null, string version = "")
        {
            int sel = 0;
            ConsoleKey key;
            bool saved = true;

            if (OperatingSystem.IsWindows())
            {
                saved = Console.CursorVisible;
                Console.CursorVisible = false;
            }

            int lastW = Console.WindowWidth;
            int lastH = Console.WindowHeight;

            // Largeur interne de la box = option la plus longue + marges
            int maxLabel = 0;
            foreach (var o in options)
                if (o.Length > maxLabel) maxLabel = o.Length;
            int boxInner = maxLabel + 8; // "  ▶  label  "

            void FullDraw()
            {
                DisplayHeader(version);
                if (!string.IsNullOrEmpty(title))
                {
                    Console.ForegroundColor = TextDim;
                    CenterWrite(title);
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            FullDraw();

            // Ligne vide pour la top-bar de la box
            Console.WriteLine();
            int menuTop = Console.CursorTop;

            // ── DrawRow : redessine UNE ligne ────────────────────────────────
            void DrawRow(int index, bool active, int w)
            {
                string arrow = active ? "▶" : " ";
                string inner = $"  {arrow}  {options[index]}  ".PadRight(boxInner);
                int boxLeft = Math.Max(0, (w - boxInner - 2) / 2);

                Console.SetCursorPosition(0, menuTop + index);
                Console.Write(new string(' ', w));
                Console.SetCursorPosition(boxLeft, menuTop + index);

                Console.ForegroundColor = TextDim;
                Console.Write("│");

                if (active)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = TextBright;
                }
                else
                {
                    Console.ForegroundColor = TextMuted;
                }
                Console.Write(inner);
                Console.ResetColor();

                Console.ForegroundColor = TextDim;
                Console.Write("│");
                Console.ResetColor();
            }

            // ── DrawBox : top et bottom de la box ────────────────────────────
            void DrawBox(int w)
            {
                int boxLeft = Math.Max(0, (w - boxInner - 2) / 2);
                string topBar = "┌" + new string('─', boxInner) + "┐";
                string botBar = "└" + new string('─', boxInner) + "┘";

                int topRow = menuTop - 1;
                if (topRow >= 0)
                {
                    Console.SetCursorPosition(0, topRow);
                    Console.Write(new string(' ', w));
                    Console.SetCursorPosition(boxLeft, topRow);
                    Console.ForegroundColor = TextDim;
                    Console.Write(topBar);
                    Console.ResetColor();
                }

                int botRow = menuTop + options.Count;
                Console.SetCursorPosition(0, botRow);
                Console.Write(new string(' ', w));
                Console.SetCursorPosition(boxLeft, botRow);
                Console.ForegroundColor = TextDim;
                Console.Write(botBar);
                Console.ResetColor();
            }

            // ── DrawHint ─────────────────────────────────────────────────────
            void DrawHint(int w)
            {
                int hintRow = menuTop + options.Count + 2;
                Console.SetCursorPosition(0, hintRow);
                Console.Write(new string(' ', w));
                string hint = "↑↓  naviguer     Enter  sélectionner";
                Console.ForegroundColor = TextDim;
                Console.SetCursorPosition(Math.Max(0, (w - hint.Length) / 2), hintRow);
                Console.Write(hint);
                Console.ResetColor();
            }

            // Dessin initial complet
            int w0 = Console.WindowWidth;
            DrawBox(w0);
            for (int i = 0; i < options.Count; i++) DrawRow(i, i == sel, w0);
            DrawHint(w0);

            try
            {
                key = ConsoleKey.NoName;
                do
                {
                    int w = Console.WindowWidth;
                    int h = Console.WindowHeight;

                    // ── Resize → full redraw ─────────────────────────────────
                    if (w != lastW || h != lastH)
                    {
                        lastW = w;
                        lastH = h;
                        FullDraw();
                        Console.WriteLine();
                        menuTop = Console.CursorTop;
                        DrawBox(w);
                        for (int i = 0; i < options.Count; i++) DrawRow(i, i == sel, w);
                        DrawHint(w);
                    }

                    // ── Poll clavier ─────────────────────────────────────────
                    if (!Console.KeyAvailable)
                    {
                        Thread.Sleep(16);
                        continue;
                    }

                    key = Console.ReadKey(true).Key;

                    // ── Navigation : seulement 2 lignes redesssinées ──────────
                    int prev = sel;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            sel = sel > 0 ? sel - 1 : options.Count - 1;
                            break;
                        case ConsoleKey.DownArrow:
                            sel = sel < options.Count - 1 ? sel + 1 : 0;
                            break;
                    }

                    if (sel != prev)
                    {
                        DrawRow(prev, false, w);
                        DrawRow(sel, true, w);
                    }

                } while (key != ConsoleKey.Enter);

                // Nettoyer la zone du menu après sélection
                int wFinal = Console.WindowWidth;
                int clearTo = menuTop + options.Count + 3;
                for (int r = menuTop - 1; r <= clearTo; r++)
                {
                    try { Console.SetCursorPosition(0, r); Console.Write(new string(' ', wFinal)); }
                    catch { }
                }
                Console.SetCursorPosition(0, menuTop - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.Clear();
                DisplayHeader(version);
                PrintWarning("Terminal trop petit. Retour au choix par défaut.");
                return 0;
            }
            finally
            {
                if (OperatingSystem.IsWindows())
                    Console.CursorVisible = saved;
                Console.ResetColor();
            }

            return sel;
        }

        // ─── TransitionEffect ────────────────────────────────────────────────────────

        public static void TransitionEffect()
        {
            Console.Clear();
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            var rand = new Random();

            ConsoleColor[] shades = { ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.DarkRed };
            DateTime end = DateTime.Now.AddMilliseconds(800);

            while (DateTime.Now < end)
            {
                Console.ForegroundColor = shades[rand.Next(shades.Length)];
                int count = (width * height) / 15;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        Console.SetCursorPosition(rand.Next(width), rand.Next(height));
                        Console.Write((char)rand.Next(33, 126));
                    }
                    catch (ArgumentOutOfRangeException) { }
                }
                Thread.Sleep(25);
            }

            // Sweep clear top-to-bottom
            Console.ResetColor();
            for (int row = 0; row < height; row++)
            {
                try { Console.SetCursorPosition(0, row); Console.Write(new string(' ', width)); }
                catch { }
                Thread.Sleep(6);
            }
            Console.Clear();
        }

        public static void QuickTransition() => Console.Clear();

        // ─── ProgressBar ─────────────────────────────────────────────────────────────

        public static void DrawProgressBar(int pct, int barWidth = 40, string? label = null)
        {
            pct = Math.Clamp(pct, 0, 100);
            int filled = barWidth * pct / 100;

            Console.CursorLeft = 0;
            Console.ForegroundColor = TextDim;
            Console.Write("  [");
            Console.ForegroundColor = AccentPrimary;
            Console.Write(new string('█', filled));
            Console.ForegroundColor = TextDim;
            Console.Write(new string('░', barWidth - filled));
            Console.Write("] ");
            Console.ForegroundColor = TextBright;
            Console.Write($"{pct,3}%");

            if (!string.IsNullOrEmpty(label))
            {
                Console.ForegroundColor = TextDim;
                Console.Write($"  {label}");
            }

            Console.ResetColor();
            if (pct >= 100) Console.WriteLine();
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private static void CenterWrite(string text)
        {
            int left = Math.Max(0, (Console.WindowWidth - text.Length) / 2);
            Console.SetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(text);
        }

        /// <summary>
        /// Barre d'infos système sous le header :
        ///   ┌─ WIN32 · ATOM v1.0.3 · username · 2026-05-17 ─┐
        /// </summary>
        private static void PrintStatusBar(string version, int w)
        {
            string os = OperatingSystem.IsWindows() ? "WIN32"
                           : OperatingSystem.IsLinux() ? "LINUX"
                                                         : "MACOS";
            string ver = string.IsNullOrEmpty(version) ? "ATOM" : $"ATOM v{version}";
            string user = Environment.UserName.ToLower();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string content = $"  {os}  ·  {ver}  ·  {user}  ·  {date}  ";

            int barW = Math.Min(w - 4, Math.Max(content.Length + 4, 48));
            int left = Math.Max(0, (w - barW) / 2);
            int inner = barW - 2;

            // Centre le texte dans la barre
            int pad = Math.Max(0, (inner - content.Length) / 2);
            string paddedContent = content.PadLeft(pad + content.Length).PadRight(inner);

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.ForegroundColor = TextDim;
            Console.WriteLine("┌" + new string('─', inner) + "┐");

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.ForegroundColor = TextDim;
            Console.Write("│");
            Console.ForegroundColor = AccentSecondary;
            Console.Write(paddedContent);
            Console.ForegroundColor = TextDim;
            Console.WriteLine("│");

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.ForegroundColor = TextDim;
            Console.WriteLine("└" + new string('─', inner) + "┘");
            Console.ResetColor();
        }
    }
}