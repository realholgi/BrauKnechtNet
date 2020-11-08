using BrauknechtStateless.PrgData;

namespace BrauknechtStatelessSpike
{
    static class Startup
    {
        private static readonly Maischprogramm M = new Maischprogramm
        {
            EinmaischTemperatur = 70, 
            Rasten = new[]
            {
                new Rast(66, 60),
                new Rast(72, 30)
            }
        };

        private static readonly Kochprogramm K = new Kochprogramm
        {
            Kochdauer = 60, 
            Hopfengaben = new[]
            {
                new Hopfengabe("Magnum", 50),
                new Hopfengabe("Amarillo", 30),
                new Hopfengabe("Simcoe", 5)
            }
        };
        
        static void Main(string[] args)
        {
            MaischautomatikRunner.Run(M);
            KochautomatikRunner.Run(K);
        }
    }
}