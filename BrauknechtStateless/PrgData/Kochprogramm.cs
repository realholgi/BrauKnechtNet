namespace BrauknechtStateless.PrgData
{
    public record Kochprogramm
    {
        public int Kochdauer { get; init; }
        public Hopfengabe[] Hopfengaben { get; init; } = System.Array.Empty<Hopfengabe>();
    }


    public record Hopfengabe
    {
        public string Name { get; private init; } = "<unbekannt>";
        public int Kochdauer { get; private init; }

        public Hopfengabe(string name, int kochdauer) => (Name, Kochdauer) = (name, kochdauer);
        public Hopfengabe(int kochdauer) => Kochdauer = kochdauer;
    }
}