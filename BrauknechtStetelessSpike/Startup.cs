using BrauknechtStateless;
using BrauknechtStateless.PrgData;

var maischprogramm = new Maischprogramm
{
    EinmaischTemperatur = 70,
    Rasten = new[]
    {
        new Rast("Kombirast", 66, 60),
        new Rast("Abmaischen", 72, 30)
    }
};
var m = new Maischautomatik(maischprogramm);
m.Einmaischen();
m.EinmaischenTemperaturErreicht();
foreach (var _ in maischprogramm.Rasten)
{
    m.Rasten();
    m.RastTemperaturErreicht();
    m.RastWartenErreicht();
}

m.Abmaischen();
m.AbmaischTemperaturErreicht();

// Console.WriteLine(_maischautomatik.ToDotGraph());
var kochprogramm = new Kochprogramm
{
    Kochdauer = 60,
    Hopfengaben = new[]
    {
        new Hopfengabe("Magnum", 50),
        new Hopfengabe("Amarillo", 30),
        new Hopfengabe("Simcoe", 5)
    }
};
var k = new Kochautomatik(kochprogramm);
k.VorderwürzeHopfungGegeben();
k.Kochen();
k.KochTemperaturErreicht();
foreach (var _ in kochprogramm.Hopfengaben)
{
    k.Hopfengabe();
    k.HopengabeErreicht();
}

k.KochEndeErreicht();

//Console.WriteLine(k.ToDotGraph());