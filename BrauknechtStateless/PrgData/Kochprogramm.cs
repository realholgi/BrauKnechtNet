namespace BrauknechtStateless.PrgData
{
    public class Kochprogramm
    {
        public int Kochdauer;
        public Hopfengabe[] Hopfengaben = System.Array.Empty<Hopfengabe>();
    }

    public class Hopfengabe
    {
        public readonly string Name;
        public readonly int Kochdauer;

        public Hopfengabe(string name, int kochdauer)
        {
            Name = name;
            Kochdauer = kochdauer;
        }
    }
    
    
}