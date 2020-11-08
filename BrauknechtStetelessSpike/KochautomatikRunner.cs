using BrauknechtStateless;
using BrauknechtStateless.PrgData;

namespace BrauknechtStatelessSpike
{
    public static class KochautomatikRunner
    {
        public static void Run(Kochprogramm prg)
        {
            var k = new Kochautomatik(prg);
            
            k.VorderwürzeHopfungGegeben();
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