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

            int startLine = Console.CursorTop;
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
                    for (int i = 0; i < options.Count; i++)
                    {
                        Console.SetCursorPosition(0, startLine + i);

                        if (i == currentSelection)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine($">> {options[i]} <<");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"   {options[i]}   ");
                            Console.ResetColor();
                        }
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
                } while (key != ConsoleKey.Enter);
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
      █████╗ ████████╗ ██████╗ ███╗   ███╗
     ██╔══██╗╚══██╔══╝██╔═══██╗████╗ ████║
     ███████║   ██║   ██║   ██║██╔████╔██║
     ██╔══██║   ██║   ██║   ██║██║╚██╔╝██║
     ██║  ██║   ██║   ╚██████╔╝██║ ╚═╝ ██║
     ╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝
            ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("        [ THE ULTIMATE MULTI-TOOL ]");
            Console.WriteLine(" ___________________________________________\n");
            Console.ResetColor();
        }

        public static void TransitionEffect()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nChargement");
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(200);
                Console.Write(".");
            }
            Console.ResetColor();
            Console.Clear();
        }
    }
}
