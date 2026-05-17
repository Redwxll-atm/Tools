using System;
using Net.Codecrete.QrCodeGenerator;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class QrCodeService
    {
        public static void HandleQrCodeGenerator()
        {
            UIHelper.DisplayHeader();
            Console.WriteLine("=== QR CODE GENERATOR ===");
            Console.Write("\n[?] Entrez le texte ou l'URL à convertir : ");
            string input = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("[!] Entrée invalide.");
                return;
            }

            try
            {
                var qr = QrCode.EncodeText(input, QrCode.Ecc.Medium);
                DisplayQrCode(qr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Erreur lors de la génération : {ex.Message}");
            }
        }

        private static void DisplayQrCode(QrCode qr)
        {
            Console.WriteLine("\n[+] QR Code généré :\n");

            // We use a small border (quiet zone)
            int border = 2;
            for (int y = -border; y < qr.Size + border; y++)
            {
                for (int x = -border; x < qr.Size + border; x++)
                {
                    bool black = qr.GetModule(x, y);
                    if (black)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("  "); // Use two spaces for a more square look
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.ResetColor();
            Console.WriteLine("\n[!] Scannez le code avec votre téléphone.");
        }
    }
}
