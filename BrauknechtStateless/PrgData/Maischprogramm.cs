namespace BrauknechtStateless.PrgData
{
    public record Maischprogramm
    {
        public double EinmaischTemperatur { get; init; }
        public Rast[] Rasten { get; init; } = System.Array.Empty<Rast>();
        public double Abmaischtemperatur { get; init; } = 78;
    }

    public record Rast
    {
        public string Name { get; private init; } = "<unbekannt>";
        public double Temperatur { get; private init; }
        public int Dauer { get; private init; }

        public Rast(string name, double temp, int dauer) => (Name, Temperatur, Dauer) = (name, temp, dauer);
        public Rast(double temp, int dauer) => (Temperatur, Dauer) = (temp, dauer);
    }
}