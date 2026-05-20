using System;
using System.Collections.Generic;
using System.Threading;

namespace Atom.Utils
{
    public static class UIHelper
    {
        // ─── Palette ─────────────────────────────────────────────────────────────────
        private static readonly ConsoleColor AccentPrimary = ConsoleColor.Red;
        private static readonly ConsoleColor AccentSecondary = ConsoleColor.DarkRed;
        private static readonly ConsoleColor TextBright = ConsoleColor.White;
        private static readonly ConsoleColor TextDim = ConsoleColor.DarkGray;
        private static readonly ConsoleColor TextMuted = ConsoleColor.Gray;
        private static readonly ConsoleColor Success = ConsoleColor.Green;
        private static readonly ConsoleColor Warning = ConsoleColor.Yellow;
        private static readonly ConsoleColor Error = ConsoleColor.Red;

        // ─── ASCII Art ────────────────────────────────────────────────────────────────
        //  Style "liquid / dripping" — effet coulant et fondu (CORRIGÉ : ATOM)
        private static readonly string[] AsciiLogo =
        {
            @"    ▄████████  ▄████████  ▄██████▄     ▄▄▄▄███▄▄▄  ",
            @"   ███    ███ ▀█████████▀ ███    ███   ▄██▀▀▀███▀▀▀██▄",
            @"   ███    ███    ███      ███    ███   ███   ███   ███",
            @"   ███    ███    ███      ███    ███   ███   ███   ███",
            @" ▀███████████    ███      ███    ███   ███   ███   ███",
            @"   ███    ███    ███      ███    ███   ███   ███   ███",
            @"   ███    ███    ███      ███    ███   ███   ███   ███",
            @"   ███    █▀     ███       ▀██████▀     ▀█   ███   █▀ ",
            @"    █            █         █            █         █",
            @"    ░            ░         ░            ░         ░",
        };

        // Tout en rouge comme demandé
        private static readonly ConsoleColor[] AsciiLogoColors =
        {
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkRed,
        };

        // Tagline affichée sous le logo
        private const string Tagline = "[ Advanced  Toolkit  for  Offensive  Mastery ]";

        // ─── Print helpers ────────────────────────────────────────────────────────────

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
            Console.Write("  [~] ");
            Console.ForegroundColor = TextMuted;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintField(string label, string value, int labelWidth = 12)
        {
            Console.ForegroundColor = TextDim;
            Console.Write($"  {label.PadRight(labelWidth)}");
            Console.ForegroundColor = AccentSecondary;
            Console.Write(" │ ");
            Console.ForegroundColor = TextBright;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        public static string? Prompt(string question)
        {
            Console.WriteLine();
            Console.ForegroundColor = AccentSecondary;
            Console.Write("  ┌─ ");
            Console.ForegroundColor = TextBright;
            Console.WriteLine(question);
            Console.ForegroundColor = AccentSecondary;
            Console.Write("  └─▶ ");
            Console.ForegroundColor = TextMuted;
            string? input = Console.ReadLine();
            Console.ResetColor();
            return input?.Trim();
        }

        public static void PressAnyKey()
        {
            Console.WriteLine();
            Console.ForegroundColor = TextDim;
            CenterWrite("── appuyez sur une touche pour continuer ──");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        // ─── Section header ───────────────────────────────────────────────────────────

        public static void PrintSectionHeader(string title)
        {
            Console.WriteLine();
            int width = Math.Min(Console.WindowWidth - 4, 54);
            string label = $"  {title.ToUpper()}  ";
            int padRight = width - label.Length;

            Console.ForegroundColor = AccentSecondary;
            Console.Write("  ┌");
            Console.ForegroundColor = AccentPrimary;
            Console.Write(new string('─', width));
            Console.ForegroundColor = AccentSecondary;
            Console.WriteLine("┐");

            Console.Write("  │");
            Console.ForegroundColor = TextBright;
            Console.Write(label);
            Console.ForegroundColor = TextDim;
            Console.Write(new string(' ', Math.Max(0, padRight)));
            Console.ForegroundColor = AccentSecondary;
            Console.WriteLine("│");

            Console.Write("  └");
            Console.ForegroundColor = AccentPrimary;
            Console.Write(new string('─', width));
            Console.ForegroundColor = AccentSecondary;
            Console.WriteLine("┘");
            Console.ResetColor();
            Console.WriteLine();
        }

        // ─── DisplayHeader ────────────────────────────────────────────────────────────

        public static void DisplayHeader(string version = "")
        {
            Console.Clear();

            if (OperatingSystem.IsWindows())
                try { Console.Title = "ATOM — Advanced Toolkit for Offensive Mastery"; } catch { }

            int w = Console.WindowWidth;

            // ── Largeur de la bordure calée sur le logo ─────────────────────
            int logoWidth = 0;
            foreach (var line in AsciiLogo)
                if (line.Length > logoWidth) logoWidth = line.Length;

            int borderW = Math.Max(logoWidth + 6, Math.Min(w - 2, 62));

            // ── Bordure haute ────────────────────────────────────────────────
            Console.ForegroundColor = AccentSecondary;
            CenterWrite("╔" + new string('═', borderW) + "╗");

            // ── Ligne vide ───────────────────────────────────────────────────
            Console.ForegroundColor = AccentSecondary;
            CenterWrite("║" + new string(' ', borderW) + "║");

            // ── Logo ─────────────────────────────────────────────────────────
            foreach (var (line, idx) in IndexedLines(AsciiLogo))
            {
                int pad = (borderW - line.Length) / 2;
                string padded = new string(' ', Math.Max(0, pad))
                              + line
                              + new string(' ', Math.Max(0, borderW - line.Length - pad));

                Console.ForegroundColor = AccentSecondary;
                PrintCenteredInBorder("║", padded, "║", AsciiLogoColors[idx]);
            }

            // ── Séparateur ────────────────────────────────────────────────────
            Console.ForegroundColor = AccentSecondary;
            CenterWrite("╠" + new string('═', borderW) + "╣");

            // ── Tagline ───────────────────────────────────────────────────────
            {
                int pad = (borderW - Tagline.Length) / 2;
                string inner = new string(' ', Math.Max(0, pad))
                             + Tagline
                             + new string(' ', Math.Max(0, borderW - Tagline.Length - pad));
                Console.ForegroundColor = AccentSecondary;
                PrintCenteredInBorder("║", inner, "║", ConsoleColor.DarkRed);
            }

            // ── Bordure basse ─────────────────────────────────────────────────
            Console.ForegroundColor = AccentSecondary;
            CenterWrite("╚" + new string('═', borderW) + "╝");
            Console.ResetColor();

            // ── Status bar ────────────────────────────────────────────────────
            Console.WriteLine();
            PrintStatusBar(version, w);
            Console.WriteLine();
        }

        // ─── SingleChoice ─────────────────────────────────────────────────────────────
        //  Améliorations vs v1 :
        //    • Numéro de ligne affiché dans chaque option  [1] … [N]
        //    • Raccourci clavier : taper le chiffre sélectionne directement
        //    • Indicateur de position  (3 / 7)  en bas à droite de la box

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

            int maxLabel = 0;
            foreach (var o in options)
                if (o.Length > maxLabel) maxLabel = o.Length;

            // largeur interne : "  [N]  label  " + marge
            int numWidth = options.Count.ToString().Length;
            int boxInner = maxLabel + numWidth + 12;

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
            Console.WriteLine();
            int menuTop = Console.CursorTop;

            // ── DrawRow ───────────────────────────────────────────────────────
            void DrawRow(int index, bool active, int w)
            {
                string num = (index + 1).ToString().PadLeft(numWidth);
                string arrow = active ? "▶" : " ";
                string label = options[index];
                string inner = $"  {arrow} [{num}]  {label}  ".PadRight(boxInner);

                int boxLeft = Math.Max(0, (w - boxInner - 2) / 2);
                SafeSetCursorPosition(0, menuTop + index);
                Console.Write(new string(' ', w));
                SafeSetCursorPosition(boxLeft, menuTop + index);

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

            // ── DrawBox ───────────────────────────────────────────────────────
            void DrawBox(int w)
            {
                int boxLeft = Math.Max(0, (w - boxInner - 2) / 2);

                // Top
                int topRow = menuTop - 1;
                if (topRow >= 0)
                {
                    SafeSetCursorPosition(0, topRow);
                    Console.Write(new string(' ', w));
                    SafeSetCursorPosition(boxLeft, topRow);
                    Console.ForegroundColor = TextDim;
                    Console.Write("┌" + new string('─', boxInner) + "┐");
                    Console.ResetColor();
                }

                // Bottom avec indicateur de position
                int botRow = menuTop + options.Count;
                SafeSetCursorPosition(0, botRow);
                Console.Write(new string(' ', w));
                SafeSetCursorPosition(boxLeft, botRow);
                Console.ForegroundColor = TextDim;

                string indicator = $" {sel + 1}/{options.Count} ";
                int barInner = boxInner - indicator.Length;
                string leftPart = new string('─', Math.Max(0, barInner));
                Console.Write("└" + leftPart);
                Console.ForegroundColor = AccentSecondary;
                Console.Write(indicator);
                Console.ForegroundColor = TextDim;
                Console.Write("┘");
                Console.ResetColor();
            }

            // ── DrawHint ──────────────────────────────────────────────────────
            void DrawHint(int w)
            {
                int hintRow = menuTop + options.Count + 2;
                SafeSetCursorPosition(0, hintRow);
                Console.Write(new string(' ', w));

                string hint = "↑↓  naviguer    [1-9]  accès direct    Enter  confirmer    Q  quitter";
                if (hint.Length > w - 4) hint = "↑↓  naviguer    Enter  confirmer    Q  quitter";

                Console.ForegroundColor = TextDim;
                SafeSetCursorPosition(Math.Max(0, (w - hint.Length) / 2), hintRow);
                Console.Write(hint);
                Console.ResetColor();
            }

            // Dessin initial
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

                    // Resize → full redraw
                    if (w != lastW || h != lastH)
                    {
                        lastW = w; lastH = h;
                        FullDraw();
                        Console.WriteLine();
                        menuTop = Console.CursorTop;
                        DrawBox(w);
                        for (int i = 0; i < options.Count; i++) DrawRow(i, i == sel, w);
                        DrawHint(w);
                    }

                    if (!Console.KeyAvailable) { Thread.Sleep(16); continue; }

                    var info = Console.ReadKey(true);
                    key = info.Key;

                    int prev = sel;

                    // Accès direct par chiffre
                    if (info.KeyChar >= '1' && info.KeyChar <= '9')
                    {
                        int digit = info.KeyChar - '1';
                        if (digit < options.Count)
                        {
                            sel = digit;
                            if (sel != prev)
                            {
                                DrawRow(prev, false, w);
                                DrawRow(sel, true, w);
                                DrawBox(w);
                            }
                        }
                        continue;
                    }

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.K:
                            sel = sel > 0 ? sel - 1 : options.Count - 1;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.J:
                            sel = sel < options.Count - 1 ? sel + 1 : 0;
                            break;
                        case ConsoleKey.Q:
                            // Quitter proprement → retourne -1
                            return -1;
                    }

                    if (sel != prev)
                    {
                        DrawRow(prev, false, w);
                        DrawRow(sel, true, w);
                        DrawBox(w); // met à jour l'indicateur de position
                    }

                } while (key != ConsoleKey.Enter);

                // Nettoyage zone menu
                int wFinal = Console.WindowWidth;
                int clearTo = menuTop + options.Count + 4;
                for (int r = menuTop - 1; r <= clearTo; r++)
                {
                    try { SafeSetCursorPosition(0, r); Console.Write(new string(' ', wFinal)); }
                    catch { }
                }
                SafeSetCursorPosition(0, menuTop - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.Clear();
                DisplayHeader(version);
                PrintWarning("Terminal trop petit — retour au choix par défaut.");
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

        // ─── TransitionEffect ─────────────────────────────────────────────────────────

        public static void TransitionEffect()
        {
            Console.Clear();
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            var rand = new Random();

            // Caractères de "bruit" bruitage ASCII
            string noise = "▓▒░█▄▀■□▪▫";
            ConsoleColor[] shades = { ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.DarkRed };
            DateTime end = DateTime.Now.AddMilliseconds(700);

            while (DateTime.Now < end)
            {
                Console.ForegroundColor = shades[rand.Next(shades.Length)];
                int count = (width * height) / 12;
                for (int i = 0; i < count; i++)
                {
                    SafeSetCursorPosition(rand.Next(width), rand.Next(height));
                    Console.Write(noise[rand.Next(noise.Length)]);
                }
                Thread.Sleep(20);
            }

            // Sweep clear (ligne par ligne)
            Console.ResetColor();
            for (int row = 0; row < height; row++)
            {
                SafeSetCursorPosition(0, row);
                Console.Write(new string(' ', width));
                Thread.Sleep(5);
            }
            Console.Clear();
        }

        public static void QuickTransition() => Console.Clear();

        // ─── ProgressBar ──────────────────────────────────────────────────────────────

        public static void DrawProgressBar(int pct, int barWidth = 40, string? label = null)
        {
            pct = Math.Clamp(pct, 0, 100);
            int filled = barWidth * pct / 100;

            Console.CursorLeft = 0;
            Console.ForegroundColor = TextDim;
            Console.Write("  ▕");
            Console.ForegroundColor = AccentPrimary;
            Console.Write(new string('█', filled));
            Console.ForegroundColor = AccentSecondary;
            Console.Write(new string('▒', barWidth - filled));
            Console.ForegroundColor = TextDim;
            Console.Write("▏ ");
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

        // ─── Spinner ──────────────────────────────────────────────────────────────────
        //  Nouveau : spinner animé pour les opérations asynchrones
        //  Usage :
        //    var cts = new CancellationTokenSource();
        //    var spin = Task.Run(() => UIHelper.Spinner("Chargement", cts.Token));
        //    // ... travail ...
        //    cts.Cancel(); await spin;

        public static void Spinner(string label, CancellationToken ct)
        {
            char[] frames = { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
            int idx = 0;

            bool savedVisible = true;
            if (OperatingSystem.IsWindows())
            {
                savedVisible = Console.CursorVisible;
                Console.CursorVisible = false;
            }

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    Console.CursorLeft = 0;
                    Console.ForegroundColor = AccentPrimary;
                    Console.Write($"  {frames[idx % frames.Length]} ");
                    Console.ForegroundColor = TextMuted;
                    Console.Write(label + "   ");
                    Console.ResetColor();
                    idx++;
                    Thread.Sleep(80);
                }
            }
            finally
            {
                Console.CursorLeft = 0;
                Console.Write(new string(' ', label.Length + 8));
                Console.CursorLeft = 0;
                if (OperatingSystem.IsWindows())
                    Console.CursorVisible = savedVisible;
                Console.ResetColor();
            }
        }

        // ─── Private helpers ──────────────────────────────────────────────────────────

        private static void SafeSetCursorPosition(int left, int top)
        {
            try
            {
                Console.SetCursorPosition(
                    Math.Clamp(left, 0, Console.WindowWidth - 1),
                    Math.Clamp(top, 0, Console.WindowHeight - 1));
            }
            catch { }
        }

        private static void CenterWrite(string text)
        {
            int left = Math.Max(0, (Console.WindowWidth - text.Length) / 2);
            SafeSetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(text);
        }

        /// <summary>Affiche une ligne encadrée de deux bornes, avec couleur centrale.</summary>
        private static void PrintCenteredInBorder(string left, string content, string right, ConsoleColor contentColor)
        {
            int w = Console.WindowWidth;
            int total = left.Length + content.Length + right.Length;
            int offset = Math.Max(0, (w - total) / 2);

            SafeSetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', offset));
            Console.ForegroundColor = AccentSecondary;
            Console.Write(left);
            Console.ForegroundColor = contentColor;
            Console.Write(content);
            Console.ForegroundColor = AccentSecondary;
            Console.WriteLine(right);
        }

        /// <summary>
        /// Barre d'état système sous le header :
        ///   ┌─ LINUX · ATOM v2.0.0 · user · 2026-05-19 ─────────────┐
        /// </summary>
        private static void PrintStatusBar(string version, int w)
        {
            string os = OperatingSystem.IsWindows() ? "WIN32"
                           : OperatingSystem.IsLinux() ? "LINUX"
                                                         : "MACOS";
            string ver = string.IsNullOrEmpty(version) ? "ATOM" : $"ATOM v{version}";
            string user = Environment.UserName.ToLower();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm");
            string content = $"  {os}  ·  {ver}  ·  {user}  ·  {date}  ·  {time}  ";

            int barW = Math.Min(w - 4, Math.Max(content.Length + 4, 52));
            int left = Math.Max(0, (w - barW) / 2);
            int inner = barW - 2;

            int pad = Math.Max(0, (inner - content.Length) / 2);
            string paddedContent = content.PadLeft(pad + content.Length).PadRight(inner);

            SafeSetCursorPosition(left, Console.CursorTop);
            Console.ForegroundColor = TextDim;
            Console.WriteLine("┌" + new string('─', inner) + "┐");

            SafeSetCursorPosition(left, Console.CursorTop);
            Console.ForegroundColor = TextDim;
            Console.Write("│");
            Console.ForegroundColor = AccentSecondary;
            Console.Write(paddedContent);
            Console.ForegroundColor = TextDim;
            Console.WriteLine("│");

            SafeSetCursorPosition(left, Console.CursorTop);
            Console.ForegroundColor = TextDim;
            Console.WriteLine("└" + new string('─', inner) + "┘");
            Console.ResetColor();
        }

        /// <summary>Enumerate avec index — remplace LINQ pour éviter la dépendance.</summary>
        private static IEnumerable<(T item, int index)> IndexedLines<T>(IEnumerable<T> source)
        {
            int i = 0;
            foreach (var item in source) yield return (item, i++);
        }
    }
}