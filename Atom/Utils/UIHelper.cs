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
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"> {options[i]} <");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"  {options[i]}  ");
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

            return currentSelection;
        }
    }
}
