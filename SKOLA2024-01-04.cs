using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace School
{
    public class SkolaDbContext : DbContext
    {
        public DbSet<Elev> Elever { get; set; }
        public DbSet<Personal> PersonalSet { get; set; }
        public DbSet<Kurs> Kurser { get; set; }
        public DbSet<Betyg> BetygSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=School;Trusted_Connection=True;");
        }

        public class Elev
        {
            public int ElevId { get; set; }
            public string? Namn { get; set; }
            public string? Personnummer { get; set; }
            public string? Klass { get; set; }
        }

        public class Personal
        {
            public int PersonalId { get; set; }
            public string? Namn { get; set; }
            public string? Befattning { get; set; }
        }

        public class Kurs
        {
            public int KursId { get; set; }
            public string? Kursnamn { get; set; }
        }

        public class Betyg
        {
            public int BetygId { get; set; }
            public int ElevId { get; set; }
            public int KursId { get; set; }
            public string? Betygsgrad { get; set; }
            public DateTime BetygDatum { get; set; }

           
            public Elev? BetygElev { get; set; }
            public Kurs? BetygKurs { get; set; }
        }

        static void Main()
        {
            using (var dbContext = new SkolaDbContext())
            {
                bool exit = false;

                while (!exit)
                { //Menyval för att välja vad som ska ses
                    Console.WriteLine("Vad önskar du se?:");
                    Console.WriteLine("1. Hämta personal");
                    Console.WriteLine("2. Hämta alla elever");
                    Console.WriteLine("3. Hämta alla elever i en viss klass");
                    Console.WriteLine("4. Hämta alla betyg som satts den senaste månaden");
                    Console.WriteLine("5. Hämta en lista med alla kurser och dess statistik");
                    Console.WriteLine("6. Lägg till nya elever");
                    Console.WriteLine("7. Lägg till ny personal");
                    Console.WriteLine("8. Avsluta");

                    Console.Write("Ange ditt val: ");
                    string val = Console.ReadLine();

                    switch (val)
                    {
                        case "1":
                            //Menyval för att välja vem som ska ses
                            Console.WriteLine("Vill du se alla anställda eller någon specifik?");
                            Console.WriteLine("1. Alla anställda");
                            Console.WriteLine("2. Lärare");
                            Console.WriteLine("3. Administratörer");
                            Console.WriteLine("4. Rektorer");
                            Console.Write("Ange ditt val: ");
                            string personalVal = Console.ReadLine();

                            switch (personalVal) //Det val som väljs  listas
                            {
                                case "1":
                                    var allaAnstallda = dbContext.PersonalSet.ToList();
                                    Console.WriteLine("Alla anställda:");
                                    foreach (var anställd in allaAnstallda)
                                    {
                                        Console.WriteLine($"{anställd.Namn} - {anställd.Befattning}");
                                    }
                                    break;
                                case "2":
                                    var lärare = dbContext.PersonalSet.Where(p => p.Befattning == "Lärare").ToList();
                                    Console.WriteLine("Alla lärare:");
                                    foreach (var lärarePerson in lärare)
                                    {
                                        Console.WriteLine($"{lärarePerson.Namn} - {lärarePerson.Befattning}");
                                    }
                                    break;
                                case "3":
                                    var admins = dbContext.PersonalSet.Where(p => p.Befattning == "Administratör").ToList();
                                    Console.WriteLine("Alla administratörer:");
                                    foreach (var admin in admins)
                                    {
                                        Console.WriteLine($"{admin.Namn} - {admin.Befattning}");
                                    }
                                    break;
                                case "4":
                                    var rektorer = dbContext.PersonalSet.Where(p => p.Befattning == "Rektor").ToList();
                                    Console.WriteLine("Alla rektorer:");
                                    foreach (var rektor in rektorer)
                                    {
                                        Console.WriteLine($"{rektor.Namn} - {rektor.Befattning}");
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Fel!. Försök igen.");
                                    break;
                            }
                            break;

                        case "2":
                            //Hämta alla elever
                            var elever = dbContext.Elever.OrderBy(e => e.Namn).ToList();
                            Console.WriteLine("Alla elever:");
                            foreach (var elev in elever)
                            {
                                Console.WriteLine($"{elev.Namn} - {elev.Personnummer} - {elev.Klass}");
                            }
                            break;

                        case "3":
                            //Hämta elever i en viss klass
                            var klasser = dbContext.Elever.Select(e => e.Klass).Distinct().ToList();

                            Console.WriteLine("Välj en klass:");
                            for (int i = 0; i < klasser.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {klasser[i]}");
                            }

                            Console.Write("Ange klassnummer: ");
                            int valdKlassIndex;
                            if (int.TryParse(Console.ReadLine(), out valdKlassIndex) && valdKlassIndex > 0 && valdKlassIndex <= klasser.Count)
                            {
                                var valdKlass = klasser[valdKlassIndex - 1];
                                var eleverIKlass = dbContext.Elever
                                    .Where(e => e.Klass == valdKlass)
                                    .OrderBy(e => e.Namn)
                                    .ToList();

                                Console.WriteLine($"Elever i klass {valdKlass}:");
                                foreach (var elev in eleverIKlass)
                                {
                                    Console.WriteLine($"{elev.Namn} - {elev.Personnummer}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Fel! Försök igen.");
                            }
                            break;

                        case "4":
                            //Hämta betyg den senaste månaden
                            var senasteManadensBetyg = dbContext.BetygSet
                                .Where(b => b.BetygDatum >= DateTime.Now.AddMonths(-1))
                                .ToList();

                            Console.WriteLine("Betyg som satts den senaste månaden:");
                            foreach (var betyg in senasteManadensBetyg)
                            {
                                Console.WriteLine($"{betyg.BetygElev.Namn} - {betyg.BetygKurs.Kursnamn} - {betyg.Betygsgrad} - {betyg.BetygDatum}");
                            }
                            break;

                        case "5":
                            //Hämta kurser och statistik
                            var kursStatistik = dbContext.Kurser
                                .Select(k => new
                                {
                                    Kursnamn = k.Kursnamn,
                                    Snittbetyg = dbContext.BetygSet.Where(b => b.KursId == k.KursId).Average(b => Convert.ToDouble(b.Betygsgrad)),
                                    HögstaBetyg = dbContext.BetygSet.Where(b => b.KursId == k.KursId).Max(b => b.Betygsgrad),
                                    LägstaBetyg = dbContext.BetygSet.Where(b => b.KursId == k.KursId).Min(b => b.Betygsgrad)
                                })
                                .ToList();


                            Console.WriteLine("Statistik för alla kurser:");
                            foreach (var kurs in kursStatistik)
                            {
                                Console.WriteLine($"{kurs.Kursnamn} - Snittbetyg: {kurs.Snittbetyg}, Högsta betyg: {kurs.HögstaBetyg}, Lägsta betyg: {kurs.LägstaBetyg}");
                            }
                            break;

                        case "6":
                            //Lägg till nya elever
                            Console.Write("Ange elevens namn: ");
                            string nyElevNamn = Console.ReadLine();
                            Console.Write("Ange elevens personnummer: ");
                            string nyElevPersonnummer = Console.ReadLine();
                            Console.Write("Ange elevens klass: ");
                            string nyElevKlass = Console.ReadLine();

                            var nyElev = new Elev { Namn = nyElevNamn, Personnummer = nyElevPersonnummer, Klass = nyElevKlass };
                            dbContext.Elever.Add(nyElev);
                            dbContext.SaveChanges();

                            Console.WriteLine("En ny elev har lagts till.");
                            break;

                        case "7":
                            //Lägg till ny personal
                            Console.Write("Ange personalens namn: ");
                            string nyPersonalNamn = Console.ReadLine();
                            Console.Write("Ange personalens befattning: ");
                            string nyPersonalBefattning = Console.ReadLine();

                            var nyPersonal = new Personal { Namn = nyPersonalNamn, Befattning = nyPersonalBefattning };
                            dbContext.PersonalSet.Add(nyPersonal);
                            dbContext.SaveChanges();

                            Console.WriteLine("Nu finns personen i systemet.");
                            break;

                        case "8":
                            exit = true;
                            break;

                        default:
                            Console.WriteLine("Fel! Försök igen.");
                            break;
                    }
                }
            }
        }
    }
}