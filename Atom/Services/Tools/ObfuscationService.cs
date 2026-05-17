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
            // 1. Advanced String Encryption (XOR with dynamic key)
            code = AdvancedEncryptCSharpStrings(code);

            // 2. Control Flow Confusion (Junk logic injection)
            code = InjectCSharpControlFlowJunk(code);

            // 3. Name Scrambling (Basic Regex-based for internal vars)
            code = ScrambleCSharpVariables(code);
            
            return "/* [!] PROTECTED BY ATOM OBFUSCATOR v2.0 [!] */\n" + code;
        }

        private static string AdvancedEncryptCSharpStrings(string code)
        {
            int key = new Random().Next(100, 255);
            return Regex.Replace(code, "\"([^\"]*)\"", m =>
            {
                string val = m.Groups[1].Value;
                if (string.IsNullOrEmpty(val)) return "\"\"";
                
                byte[] bytes = Encoding.UTF8.GetBytes(val);
                for (int i = 0; i < bytes.Length; i++) bytes[i] ^= (byte)key;
                
                string b64 = Convert.ToBase64String(bytes);
                return $"Encoding.UTF8.GetString(Convert.FromBase64String(\"{b64}\").Select(b => (byte)(b ^ {key})).ToArray())";
            });
        }

        private static string InjectCSharpControlFlowJunk(string code)
        {
            string junk = "\n#region Internal_Logic\n[System.Runtime.CompilerServices.CompilerGenerated]\nprivate static void _0x" + Guid.NewGuid().ToString("N").Substring(0, 8) + "() { \n" +
                          "  var _t = DateTime.Now.Ticks; \n" +
                          "  if (_t % 2 == 0) { /* Dead code branch */ } \n" +
                          "}\n#endregion\n";
            return code.Insert(code.LastIndexOf('}') > 0 ? code.LastIndexOf('}') : code.Length, junk);
        }

        private static string ScrambleCSharpVariables(string code)
        {
            // Simple logic to replace common variable patterns with unreadable ones
            string[] patterns = { "var ", "string ", "int ", "bool " };
            foreach (var p in patterns)
            {
                code = Regex.Replace(code, p + "([a-zA-Z_][a-zA-Z0-9_]*)", m => 
                    p + "_0x" + Guid.NewGuid().ToString("N").Substring(0, 10));
            }
            return code;
        }
        #endregion

        #region Python Engine
        private static string ObfuscatePython(string code)
        {
            // Layer 1: Variable Scrambling
            code = Regex.Replace(code, @"(\w+)\s*=\s*", m => $"_0x{Guid.NewGuid().ToString("N").Substring(0, 8)} = ");

            // Layer 2: Multi-pass Base64 + Zlib Compression
            string current = code;
            for (int i = 0; i < 3; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(current);
                string b64 = Convert.ToBase64String(bytes);
                current = $"import base64;exec(base64.b64decode('{b64}').decode())";
            }

            return "# [!] ATOM POLYMORPHIC ENGINE [!]\n" + current;
        }
        #endregion

        #region JavaScript Engine
        private static string ObfuscateJavaScript(string code)
        {
            // Hex-encoding with an evaluation wrapper that looks like junk
            StringBuilder hex = new StringBuilder();
            foreach (char c in code) hex.Append("\\x" + ((int)c).ToString("X2"));

            string junkPrefix = "var _0x" + Guid.NewGuid().ToString("N").Substring(0, 6) + "=['\\x65\\x76\\x61\\x6c'];";
            return $"{junkPrefix}(function(_0x1,_0x2){{return _0x1[_0x2]}})(window,_0x{junkPrefix.Substring(7,6)}[0])(\"{hex}\");";
        }
        #endregion
    }
}
