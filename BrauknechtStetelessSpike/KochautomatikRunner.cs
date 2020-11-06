using BrauknechtStateless;

namespace BrauknechtStatelessSpike
{
    public class KochautomatikRunner
    {
        private readonly Kochautomatik _k;

        public KochautomatikRunner(Kochautomatik k)
        {
            _k = k;
        }

        public void Run(Kochprogramm prg)
        {
            _k.Prg = prg;
            
            _k.VorderwürzeHopfungGegeben();
            _k.Kochen();
            _k.KochTemperaturErreicht();

            foreach (var _ in prg.Hopfengaben)
            {
                _k.Hopfengabe();
                _k.HopengabeErreicht();
            }

            _k.KochEndeErreicht();

            //Console.WriteLine(k.ToDotGraph());
        }
    }
}