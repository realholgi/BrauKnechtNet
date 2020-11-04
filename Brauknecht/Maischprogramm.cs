namespace Brauknecht
{
    public class Maischprogramm
    {
        public double EinmaischTemperatur;
        public Rast[] Rasten = new Rast[0];
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