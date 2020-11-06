using BrauknechtStateless;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BrauknechtStatelessTests
{
    public class KochautomatikTests
    {
        private readonly ILogger<Kochautomatik> _logger = Mock.Of<ILogger<Kochautomatik>>();
        
        private Kochprogramm _prg;
        
        private Kochautomatik CreateTestKochautomatik()
        {
            _prg = new Kochprogramm
            {
                Kochdauer = 60, 
                Hopfengaben = new[]
                {
                    new Hopfengabe("Magnum", 50),
                    new Hopfengabe("Amarillo", 30),
                    new Hopfengabe("Simcoe", 5)
                }
            };
            return new Kochautomatik(_logger) {Prg = _prg};
        }
        
        [Fact]
        public void IsOffAtStartTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act

            // Assert
            Assert.Equal(Kochautomatik.State.Aus, k.CurrentState);
        }
        
        [Fact]
        public void VorderwürzeHopfungOkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();

            // Assert
            Assert.Equal(Kochautomatik.State.VorderwürzeHopfung, k.CurrentState);
        }
        
        [Fact]
        public void KochenOhneVorderwürzeHopfungOkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.Kochen();
            
            // Assert
            Assert.Equal(Kochautomatik.State.KochenAufheizen, k.CurrentState);
        }

        [Fact]
        public void KochenMitVorderwürzeHopfungOkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            
            // Assert
            Assert.Equal(Kochautomatik.State.KochenAufheizen, k.CurrentState);
        }
        
        [Fact]
        public void KochtemperaturerreichtOkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            // Assert
            Assert.Equal(Kochautomatik.State.Kochen, k.CurrentState);
        }
        
        [Fact]
        public void KochenOhneHopfengabeOkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            k.Kochen();

            // Assert
            Assert.Equal(Kochautomatik.State.Kochen, k.CurrentState);
        }
        
        [Fact]
        public void KochenOhneHopfengabeFertigOkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            k.Kochen();
            k.KochEndeErreicht();
            
            // Assert
            Assert.Equal(Kochautomatik.State.Aus, k.CurrentState);
        }
        
        [Fact]
        public void HopfengabeOkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            k.Hopfengabe();

            // Assert
            Assert.Equal(Kochautomatik.State.HopfengabeWarten, k.CurrentState);
        }
        
        [Fact]
        public void HopfengabeErreichtTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            k.Hopfengabe();
            k.HopengabeErreicht();

            // Assert
            Assert.Equal(Kochautomatik.State.HopfenGeben, k.CurrentState);
        }
        
        [Fact]
        public void HopfengabeWeiterTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            k.Hopfengabe();
            k.HopengabeErreicht();
            
            k.Hopfengabe();
            
            // Assert
            Assert.Equal(Kochautomatik.State.HopfengabeWarten, k.CurrentState);
        }
        
        [Fact]
        public void KochenWeiterTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            k.Hopfengabe();
            k.HopengabeErreicht();
            
            k.Kochen();
            
            // Assert
            Assert.Equal(Kochautomatik.State.Kochen, k.CurrentState);
        }
        
        [Fact]
        public void Kochende1OkTest()
        {
            // Arrange
            var k = CreateTestKochautomatik();

            // Act
            k.VorderwürzeHopfungGegeben();
            k.Kochen();
            k.KochTemperaturErreicht();
            
            k.Hopfengabe();
            k.HopengabeErreicht();
            
            k.KochEndeErreicht();
            
            // Assert
            Assert.Equal(Kochautomatik.State.Aus, k.CurrentState);
        }
    }
}