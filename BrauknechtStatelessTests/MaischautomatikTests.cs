using BrauknechtStateless;
using BrauknechtStateless.PrgData;
using Xunit;

namespace BrauknechtStatelessTests
{
    public class MaischautomatikTests
    {
        private Maischprogramm _prg = new Maischprogramm();

        private Maischautomatik CreateTestMaischautomatik()
        {
            _prg = new Maischprogramm
            {
                EinmaischTemperatur = 70,
                Rasten = new[]
                {
                    new Rast(66, 60),
                    new Rast(72, 30)
                }
            };
            return new Maischautomatik(_prg);
        }

        [Fact]
        public void IsOffAtStartTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act

            // Assert
            Assert.Equal(0, m.TempSoll);
            Assert.Equal(Maischautomatik.State.Aus, m.CurrentState);
        }

        [Fact]
        public void EinmaischenOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();

            // Assert
            Assert.Equal(_prg.EinmaischTemperatur, m.TempSoll);
            Assert.Equal(Maischautomatik.State.EinmaischenAufheizen, m.CurrentState);
        }

        [Fact]
        public void EinmaischenTemperaturErreichtOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();

            // Assert
            Assert.Equal(_prg.EinmaischTemperatur, m.TempSoll);
            Assert.Equal(Maischautomatik.State.EinmaischenFertig, m.CurrentState);
        }

        [Fact]
        public void RastAufheizenOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();
            m.Rasten();

            // Assert
            Assert.Equal(_prg.Rasten[0].Temperatur, m.TempSoll);
            Assert.Equal(_prg.Rasten[0].Dauer, m.DauerSoll);
            Assert.Equal(Maischautomatik.State.RastAufheizen, m.CurrentState);
        }

        [Fact]
        public void RastWartenOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();
            m.Rasten();
            m.RastTemperaturErreicht();

            // Assert
            Assert.Equal(_prg.Rasten[0].Temperatur, m.TempSoll);
            Assert.Equal(_prg.Rasten[0].Dauer, m.DauerSoll);
            Assert.Equal(Maischautomatik.State.RastWarten, m.CurrentState);
        }

        [Fact]
        public void RastWartenFertigOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();
            m.Rasten();
            m.RastTemperaturErreicht();
            m.RastWartenErreicht();

            // Assert
            Assert.Equal(_prg.Rasten[0].Temperatur, m.TempSoll);
            Assert.Equal(_prg.Rasten[0].Dauer, m.DauerSoll);
            Assert.Equal(Maischautomatik.State.RastFertig, m.CurrentState);
        }

        [Fact]
        public void NextRastOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();
            m.Rasten();
            m.RastTemperaturErreicht();
            m.RastWartenErreicht();
            m.Rasten();

            // Assert
            Assert.Equal(_prg.Rasten[1].Temperatur, m.TempSoll);
            Assert.Equal(_prg.Rasten[1].Dauer, m.DauerSoll);
            Assert.Equal(Maischautomatik.State.RastAufheizen, m.CurrentState);
        }

        [Fact]
        public void LastRastOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();
            foreach (var _ in _prg.Rasten)
            {
                m.Rasten();
                m.RastTemperaturErreicht();
                m.RastWartenErreicht();
            }

            // Assert
            Assert.Equal(_prg.Rasten[^1].Temperatur, m.TempSoll);
            Assert.Equal(_prg.Rasten[^1].Dauer, m.DauerSoll);
            Assert.Equal(Maischautomatik.State.RastFertig, m.CurrentState);
        }

        [Fact]
        public void AbmaischenOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();
            foreach (var _ in _prg.Rasten)
            {
                m.Rasten();
                m.RastTemperaturErreicht();
                m.RastWartenErreicht();
            }

            m.Abmaischen();

            // Assert
            Assert.Equal(_prg.Abmaischtemperatur, m.TempSoll);
            Assert.Equal(0, m.DauerSoll);
            Assert.Equal(Maischautomatik.State.AbmaischenAufheizen, m.CurrentState);
        }

        [Fact]
        public void AbmaischenFertigOkTest()
        {
            // Arrange
            var m = CreateTestMaischautomatik();

            // Act
            m.Einmaischen();
            m.EinmaischenTemperaturErreicht();
            foreach (var _ in _prg.Rasten)
            {
                m.Rasten();
                m.RastTemperaturErreicht();
                m.RastWartenErreicht();
            }

            m.Abmaischen();
            m.AbmaischTemperaturErreicht();

            // Assert
            Assert.Equal(0, m.TempSoll);
            Assert.Equal(0, m.DauerSoll);
            Assert.Equal(Maischautomatik.State.Aus, m.CurrentState);
        }
    }
}