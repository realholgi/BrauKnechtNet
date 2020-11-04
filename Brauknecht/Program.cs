using System;
using System.IO;
using static Brauknecht.Maischautomatik;

namespace Brauknecht
{
    class Program
    {
        static void Main(string[] args)
        {
            Maischen();
            //Kochen();
        }

        private static void Maischen()
        {
            Console.WriteLine("Maischautomatik");

            var prg = new Maischprogramm
            {
                EinmaischTemperatur = 70, 
                Rasten = new[]
                {
                    new Rast(66, 60),
                    new Rast(72, 30)
                }
            };

            var m = new Maischautomatik(prg);

            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();

            foreach (var _ in prg.Rasten)
            {
                m.Rasten();
                m.RastTemperaturErreicht();
                m.RastWartenErreicht();
            }

            m.Abmaischen();
            m.AbmaischTemperaturErreicht();
            
            // Console.WriteLine(m.ToDotGraph());
        }

        private static void Kochen()
        {
            Console.WriteLine("Kochautomatik");

            var k = new Kochautomatik(60);
            k.VorderwürzeGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();

            Hopfengabe(50);
            Hopfengabe(30);
            Hopfengabe(5);

            k.KochEndeErreicht();

            //Console.WriteLine(k.ToDotGraph());

            void Hopfengabe(int dauer)
            {
                k.Hopfengabe(dauer);
                k.HopengabeErreicht();
            }
        }
    }
}