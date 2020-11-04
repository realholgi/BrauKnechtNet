﻿using System;
using static Brauknecht.Maischautomatik;

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

            var prg = new Kochprogramm
            {
                Kochdauer = 60, 
                Hopfengaben = new[]
                {
                    new Hopfengabe("Magnum", 50),
                    new Hopfengabe("Amarillo", 30),
                    new Hopfengabe("Simcoe", 5)
                }
            };
            
            var k = new Kochautomatik(prg);
            
            k.VorderwürzeGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();

            foreach (var _ in prg.Hopfengaben)
            {
                k.Hopfengabe();
                k.HopengabeErreicht();
            }

            k.KochEndeErreicht();

            //Console.WriteLine(k.ToDotGraph());
        }
    }
}