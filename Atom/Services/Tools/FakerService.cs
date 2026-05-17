using System;
using System.Collections.Generic;
using System.Linq;

namespace Atom.Services.Tools
{
    public static class FakerService
    {
        public static void HandleFakerMenu()
        {
            var options = new List<string> 
            { 
                "Fake Identity Generator", 
                "Fake Credit Card Generator", 
                "Fake Token Generator (Visual Only)", 
                "Retour" 
            };
            
            int choice = Atom.Utils.UIHelper.SingleChoice(options);
            Console.Clear();

            switch (choice)
            {
                case 0: GenerateFakeIdentity(); break;
                case 1: GenerateFakeCreditCard(); break;
                case 2: GenerateFakeToken(); break;
            }
            if (choice != 3) { Console.WriteLine("\nAppuyez sur une touche..."); Console.ReadKey(); }
        }

        private static void GenerateFakeIdentity()
        {
            string[] names = { "Jean", "Marie", "Pierre", "Sophie", "Lucas", "Emma", "Thomas", "Julie" };
            string[] lastNames = { "Dupont", "Martin", "Bernard", "Thomas", "Petit", "Robert", "Richard", "Durand" };
            string[] cities = { "Paris", "Lyon", "Marseille", "Bordeaux", "Lille", "Nantes", "Strasbourg" };
            
            Random r = new Random();
            Console.WriteLine("=== FAKE IDENTITY ===");
            Console.WriteLine($"Nom      : {names[r.Next(names.Length)]} {lastNames[r.Next(lastNames.Length)]}");
            Console.WriteLine($"Âge      : {r.Next(18, 65)} ans");
            Console.WriteLine($"Ville    : {cities[r.Next(cities.Length)]}");
            Console.WriteLine($"Email    : {names[r.Next(names.Length)].ToLower()}.{lastNames[r.Next(lastNames.Length)].ToLower()}@fake.com");
        }

        private static void GenerateFakeCreditCard()
        {
            Random r = new Random();
            string number = "4539 " + r.Next(1000, 9999) + " " + r.Next(1000, 9999) + " " + r.Next(1000, 9999);
            Console.WriteLine("=== FAKE CREDIT CARD ===");
            Console.WriteLine($"Type     : Visa");
            Console.WriteLine($"Numéro   : {number}");
            Console.WriteLine($"Expire   : {r.Next(1, 13):00}/{r.Next(25, 30)}");
            Console.WriteLine($"CVV      : {r.Next(100, 999)}");
        }

        private static void GenerateFakeToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._-";
            Random r = new Random();
            string token = "MT" + new string(Enumerable.Repeat(chars, 22).Select(s => s[r.Next(s.Length)]).ToArray()) + "." +
                           new string(Enumerable.Repeat(chars, 6).Select(s => s[r.Next(s.Length)]).ToArray()) + "." +
                           new string(Enumerable.Repeat(chars, 27).Select(s => s[r.Next(s.Length)]).ToArray());
            
            Console.WriteLine("=== FAKE TOKEN ===");
            Console.WriteLine($"Token: {token}");
            Console.WriteLine("\n[!] Ce token est totalement faux et sert uniquement de test visuel.");
        }
    }
}
