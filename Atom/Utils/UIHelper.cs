using System;
using System.Collections.Generic;

namespace Atom.Utils
{
    public static class UIHelper
    {
        public static int SingleChoice(List<string> options)
        {
            int currentSelection = 0;
            ConsoleKey key;

            bool originalCursorVisible = true;
            if (OperatingSystem.IsWindows())
            {
                originalCursorVisible = Console.CursorVisible;
                Console.CursorVisible = false;
            }

            try
            {
                do
                {
                    int startLine = Console.CursorTop;
                    int windowWidth = Console.WindowWidth;

                    for (int i = 0; i < options.Count; i++)
                    {
                        string text = i == currentSelection ? $">> {options[i]} <<" : $"   {options[i]}   ";
                        int left = Math.Max(0, (windowWidth - text.Length) / 2);
                        
                        Console.SetCursorPosition(left, startLine + i);

                        if (i == currentSelection)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(text);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(text);
                            Console.ResetColor();
                        }
                        
                        // Clear the rest of the line in case of resize
                        int remaining = windowWidth - left - text.Length;
                        if (remaining > 0) Console.Write(new string(' ', remaining));
                    }

                    key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            currentSelection = (currentSelection > 0) ? currentSelection - 1 : options.Count - 1;
                            break;
                        case ConsoleKey.DownArrow:
                            currentSelection = (currentSelection < options.Count - 1) ? currentSelection + 1 : 0;
                            break;
                    }
                    
                    // Reset cursor to start of menu for redraw
                    Console.SetCursorPosition(0, startLine);
                } while (key != ConsoleKey.Enter);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Fallback for terminals that don't support cursor positioning well
                Console.Clear();
                DisplayHeader();
                Console.WriteLine("Terminal size too small or unsupported. Please use arrows and Enter.");
                return 0; 
            }
            finally
            {
                if (OperatingSystem.IsWindows())
                {
                    Console.CursorVisible = originalCursorVisible;
                }
            }

            Console.ResetColor();
            return currentSelection;
        }

        public static void DisplayHeader()
        {
            Console.Clear();
            int width = Console.WindowWidth;
            Console.ForegroundColor = ConsoleColor.Red;
            
            string[] ascii = {
                "█████╗ ████████╗ ██████╗ ███╗   ███╗",
                "██╔══██╗╚══██╔══╝██╔═══██╗████╗ ████║",
                "███████║   ██║   ██║   ██║██╔████╔██║",
                "██╔══██║   ██║   ██║   ██║██║╚██╔╝██║",
                "██║  ██║   ██║   ╚██████╔╝██║ ╚═╝ ██║",
                "╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝"
            };

            foreach (string line in ascii)
            {
                int left = Math.Max(0, (width - line.Length) / 2);
                Console.SetCursorPosition(left, Console.CursorTop);
                Console.WriteLine(line);
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            string subtitle = "[ THE ULTIMATE MULTI-TOOL ]";
            int subLeft = Math.Max(0, (width - subtitle.Length) / 2);
            Console.SetCursorPosition(subLeft, Console.CursorTop);
            Console.WriteLine(subtitle);
            
            Console.WriteLine(new string('_', width));
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void TransitionEffect()
        {
            Console.Clear();
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            Random rand = new Random();
            
            int[] columns = new int[width];
            for (int i = 0; i < width; i++) columns[i] = rand.Next(height);

            Console.ForegroundColor = ConsoleColor.Red;
            DateTime end = DateTime.Now.AddSeconds(1);

            while (DateTime.Now < end)
            {
                for (int x = 0; x < width; x++)
                {
                    if (rand.Next(10) > 7) // Randomize rain density
                    {
                        Console.SetCursorPosition(x, columns[x]);
                        Console.Write((char)rand.Next(33, 126)); // Random ASCII chars
                        
                        columns[x]++;
                        if (columns[x] >= height)
                        {
                            columns[x] = 0;
                        }

                        // Clear the character above to simulate movement
                        int prevY = columns[x] - 1 < 0 ? height - 1 : columns[x] - 1;
                        Console.SetCursorPosition(x, prevY);
                        Console.Write(" ");
                    }
                }
                Thread.Sleep(20);
            }
            
            Console.ResetColor();
            Console.Clear();
        }
    }
}
