using System;

namespace Brauknecht
{
    class Program
    {
        static void Main(string[] args)
        {
            Maischen();
            Kochen();
        }

        private static void Maischen()
        {
            Console.WriteLine("Maischautomatik");
            
            var m = new Maischautomatik();
            m.Einmaischen(70.0);
            m.EinmaischenTemperaturErreicht();

            Rast(66, 60);
            Rast(72.0, 30);

            m.Abmaischen();
            m.AbmaischTemperaturErreicht();

            //Console.WriteLine(m.ToDotGraph());

            void Rast(double temp, int dauer)
            {
                m.Rasten(temp, dauer);
                m.RastTemperaturErreicht();
                m.RastWartenErreicht();
            }
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