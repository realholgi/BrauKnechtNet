using BrauknechtStateless;
using BrauknechtStateless.PrgData;

namespace BrauknechtStatelessSpike
{
    public static class MaischautomatikRunner
    {
        public static void Run(Maischprogramm prg)
        {
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

            // Console.WriteLine(_maischautomatik.ToDotGraph());
        }
    }
}