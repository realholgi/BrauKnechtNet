namespace BrauknechtStateless.PrgData
{
    public class Maischprogramm
    {
        public double EinmaischTemperatur;
        public Rast[] Rasten = System.Array.Empty<Rast>();
        public double Abmaischtemperatur = 78;
    }

    public class Rast
    {
        public readonly double Temperatur;
        public readonly int Dauer;

        public Rast(double temp, int dauer)
        {
            Temperatur = temp;
            Dauer = dauer;
        }
    }
}