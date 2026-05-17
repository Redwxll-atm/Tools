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
            
            Console.ForegroundColor = ConsoleColor.Red;
            DateTime end = DateTime.Now.AddSeconds(1);

            while (DateTime.Now < end)
            {
                // Increase density by drawing multiple characters per frame
                for (int i = 0; i < (width * height) / 20; i++) 
                {
                    int x = rand.Next(width);
                    int y = rand.Next(height);
                    
                    try 
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write((char)rand.Next(33, 126));
                    }
                    catch (ArgumentOutOfRangeException) { /* Ignore resize issues during transition */ }
                }
                Thread.Sleep(30);
                
                // Optional: slight fade effect by clearing a few random spots
                for (int i = 0; i < (width * height) / 40; i++)
                {
                    int x = rand.Next(width);
                    int y = rand.Next(height);
                    try 
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ");
                    }
                    catch (ArgumentOutOfRangeException) { }
                }
            }
            
            Console.ResetColor();
            Console.Clear();
        }
    }
}
