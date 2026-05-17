using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class ObfuscationService
    {
        public static void HandleObfuscationMenu()
        {
            var options = new List<string>
            {
                "Obfuscate C# (Advanced)",
                "Obfuscate Python (Base64/Exec)",
                "Obfuscate JavaScript (Hex/Scramble)",
                "Retour"
            };

            while (true)
            {
                UIHelper.DisplayHeader();
                int choice = UIHelper.SingleChoice(options);
                Console.Clear();

                if (choice == 3) break;

                Console.WriteLine("=== OBFUSCATION ENGINE ===");
                Console.WriteLine("Entrez le chemin du fichier ou collez le code directement (tapez 'END' sur une nouvelle ligne pour terminer) :");
                
                string input = GetUserInput();
                if (string.IsNullOrWhiteSpace(input)) continue;

                string result = "";
                string language = "";

                switch (choice)
                {
                    case 0:
                        result = ObfuscateCSharp(input);
                        language = "cs";
                        break;
                    case 1:
                        result = ObfuscatePython(input);
                        language = "py";
                        break;
                    case 2:
                        result = ObfuscateJavaScript(input);
                        language = "js";
                        break;
                }

                DisplayResult(result, language);
            }
        }

        private static string GetUserInput()
        {
            StringBuilder sb = new StringBuilder();
            string? line;
            while ((line = Console.ReadLine()) != "END")
            {
                if (line == null) break;
                sb.AppendLine(line);
            }
            string input = sb.ToString().Trim();

            if (File.Exists(input))
            {
                try { return File.ReadAllText(input); }
                catch { return input; }
            }
            return input;
        }

        private static void DisplayResult(string result, string ext)
        {
            Console.Clear();
            UIHelper.DisplayHeader();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Obfuscation terminée avec succès !");
            Console.ResetColor();
            Console.WriteLine("\n--- CODE OBFUSQUÉ ---");
            Console.WriteLine(result.Length > 1000 ? result.Substring(0, 1000) + "..." : result);
            Console.WriteLine("----------------------");

            Console.WriteLine("\n1. Sauvegarder dans un fichier");
            Console.WriteLine("2. Retour");
            
            if (Console.ReadLine() == "1")
            {
                string path = $"obfuscated_{DateTime.Now:yyyyMMddHHmmss}.{ext}";
                File.WriteAllText(path, result);
                Console.WriteLine($"[+] Enregistré sous: {path}");
                Console.ReadKey();
            }
        }

        #region C# Engine
        private static string ObfuscateCSharp(string code)
        {
            // 1. Advanced String Encryption (Using fully qualified names for zero-dependency)
            code = AdvancedEncryptCSharpStrings(code);

            // 2. Control Flow Confusion (Inject inside a dummy class to avoid scope issues)
            code = InjectCSharpSafetyJunk(code);

            return "/* [!] PROTECTED BY ATOM OBFUSCATOR [!] */\nusing System.Linq;\n" + code;
        }

        private static string AdvancedEncryptCSharpStrings(string code)
        {
            int key = new Random().Next(100, 255);
            return Regex.Replace(code, "\"([^\"]*)\"", m =>
            {
                string val = m.Groups[1].Value;
                if (string.IsNullOrEmpty(val) || val.Length < 2) return m.Value; // Don't encrypt very short strings/empty
                
                byte[] bytes = Encoding.UTF8.GetBytes(val);
                for (int i = 0; i < bytes.Length; i++) bytes[i] ^= (byte)key;
                
                string b64 = Convert.ToBase64String(bytes);
                return $"System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(\"{b64}\").Select(b => (byte)(b ^ {key})).ToArray())";
            });
        }

        private static string InjectCSharpSafetyJunk(string code)
        {
            string junkClass = $"\ninternal static class _0x{Guid.NewGuid().ToString("N").Substring(0, 8)} {{ \n" +
                               "  public static void Initialize() { var _t = System.DateTime.Now.Ticks; if (_t == 0) System.Console.WriteLine(\"Init\"); }\n" +
                               "}\n";
            return code + junkClass;
        }

        private static string ScrambleCSharpVariables(string code)
        {
            // Disabled variable scrambling for now to ensure 100% executability without a Roslyn parser
            return code;
        }
        #endregion

        #region Python Engine
        private static string ObfuscatePython(string code)
        {
            // Use a mapping to ensure consistency (only for local-looking variables)
            var map = new Dictionary<string, string>();
            string processed = Regex.Replace(code, @"\b([a-z_][a-z0-9_]{3,})\b", m => {
                string name = m.Groups[1].Value;
                // Avoid keywords and common built-ins
                if (IsPythonKeyword(name)) return name;
                
                if (!map.ContainsKey(name))
                    map[name] = "_0x" + Guid.NewGuid().ToString("N").Substring(0, 8);
                return map[name];
            });

            // Layer 2: Multi-pass Base64
            string current = processed;
            for (int i = 0; i < 2; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(current);
                string b64 = Convert.ToBase64String(bytes);
                current = $"import base64;exec(base64.b64decode('{b64}').decode('utf-8'))";
            }

            return "# [!] ATOM POLYMORPHIC ENGINE [!]\n" + current;
        }

        private static bool IsPythonKeyword(string word)
        {
            string[] keywords = { "import", "from", "def", "class", "return", "if", "else", "elif", "for", "while", "break", "continue", "print", "input", "range", "len", "open", "exec", "eval", "base64" };
            return Array.Exists(keywords, k => k == word);
        }
        #endregion

        #region JavaScript Engine
        private static string ObfuscateJavaScript(string code)
        {
            StringBuilder hex = new StringBuilder();
            foreach (char c in code) hex.Append("\\x" + ((int)c).ToString("X2"));

            // Use a self-contained execution block that works in both Node and Browser
            return $";(function(){{ var _e = (typeof window !== 'undefined' ? window : global).eval; _e(\"{hex}\"); }})();";
        }
        #endregion
    }
}
