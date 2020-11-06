using BrauknechtStateless;

namespace BrauknechtStatelessSpike
{
    public class MaischautomatikRunner
    {
        private readonly Maischautomatik _m;

        public MaischautomatikRunner(Maischautomatik m)
        {
            _m = m;
        }

        public void Run(Maischprogramm prg)
        {
            _m.Prg = prg;

            _m.Einmaischen();
            _m.EinmaischenTemperaturErreicht();

            foreach (var _ in prg.Rasten)
            {
                _m.Rasten();
                _m.RastTemperaturErreicht();
                _m.RastWartenErreicht();
            }

            _m.Abmaischen();
            _m.AbmaischTemperaturErreicht();

            // Console.WriteLine(_maischautomatik.ToDotGraph());
        }
    }
}