using BrauknechtStateless.PrgData;
using Stateless;
using Stateless.Graph;

namespace BrauknechtStateless
{
    public class Maischautomatik
    {
        private readonly Maischprogramm _prg;
        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private enum Trigger
        {
            EinmaischenStart,
            EinmaischenTemperaturErreicht,
            RastAufheizenStart,
            RastTemperaturErreicht,
            RastWartenErreicht,
            AbmaischenStart,
            AbmaischTemperaturErreicht
        }

        public enum State
        {
            Aus,
            EinmaischenAufheizen,
            EinmaischenFertig,
            RastAufheizen,
            RastWarten,
            RastFertig,
            AbmaischenAufheizen
        }

        private State _state = State.Aus;
        private readonly StateMachine<State, Trigger> _machine;
        
        public State CurrentState => _state;
        public double TempSoll => _tempSoll;
        private double _tempSoll;

        public int DauerSoll => _dauerSoll;
        private int _dauerSoll;
        private int _index;

        public Maischautomatik(Maischprogramm prg)
        {
            _prg = prg;

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _machine.Configure(State.Aus)
                .Permit(Trigger.EinmaischenStart, State.EinmaischenAufheizen);

            _machine.Configure(State.EinmaischenAufheizen)
                .OnEntry(OnEinmaischenAufheizen)
                .Permit(Trigger.EinmaischenTemperaturErreicht, State.EinmaischenFertig);

            _machine.Configure(State.EinmaischenFertig)
                .OnEntry(OnEinmaischenFertig)
                .Permit(Trigger.RastAufheizenStart, State.RastAufheizen);
                
            _machine.Configure(State.RastAufheizen)
                .OnEntry(() => OnRast(_index++))
                .Permit(Trigger.RastTemperaturErreicht, State.RastWarten);

            _machine.Configure(State.RastWarten)
                .OnEntry(OnRastWarten)
                .Permit(Trigger.RastWartenErreicht, State.RastFertig);

            _machine.Configure(State.RastFertig)
                .OnEntry(OnRastFertig)
                .Permit(Trigger.RastAufheizenStart, State.RastAufheizen)
                .Permit(Trigger.AbmaischenStart, State.AbmaischenAufheizen);

            _machine.Configure(State.AbmaischenAufheizen)
                .OnEntry(OnAbmaischenAufheizen)
                .OnExit(OnAbmaischenFertig)
                .Permit(Trigger.AbmaischTemperaturErreicht, State.Aus);

            _machine.OnTransitioned(t => 
                Logger.Debug(
                    $"OnTransitioned: {t.Source} -> {t.Destination} via {t.Trigger}({string.Join(", ", t.Parameters)})"));
        }

        // Einmaischen
        public void Einmaischen()
        {
            _machine.Fire(Trigger.EinmaischenStart);
        }

        private void OnEinmaischenAufheizen()
        {
            _tempSoll = _prg.EinmaischTemperatur;
            _dauerSoll = 0;
            Logger.Info($"Setze Einmaischen auf {_tempSoll}°C");
        }

        public void EinmaischenTemperaturErreicht()
        {
            _machine.Fire(Trigger.EinmaischenTemperaturErreicht);
        }

        private void OnEinmaischenFertig()
        {
            Logger.Info("Eingemaischt!");
        }


        // Rasten
        public void Rasten()
        {
            _machine.Fire(Trigger.RastAufheizenStart);
        }

        private void OnRast(int index)
        {
            _tempSoll = _prg.Rasten[index].Temperatur;
            _dauerSoll = _prg.Rasten[index].Dauer;
            Logger.Info($"Definiere Rast: {_tempSoll}°C, {_dauerSoll} min");
        }

        public void RastTemperaturErreicht()
        {
            _machine.Fire(Trigger.RastTemperaturErreicht);
        }

        private void OnRastWarten()
        {
            Logger.Info($"Rasttemperatur erreicht. Nun wird {_dauerSoll} min gewartet...");
        }

        public void RastWartenErreicht()
        {
            _machine.Fire(Trigger.RastWartenErreicht);
        }

        private void OnRastFertig()
        {
            Logger.Info("Rastdauer erreicht. Rast fertig!");
        }

        //
        // Abmaischen
        //

        public void Abmaischen()
        {
            _machine.Fire(Trigger.AbmaischenStart);
        }

        private void OnAbmaischenAufheizen()
        {
            _tempSoll = _prg.Abmaischtemperatur;
            _dauerSoll = 0;
            Logger.Info($"Setze Abmaischen auf {_tempSoll}°C");
        }

        public void AbmaischTemperaturErreicht()
        {
            _machine.Fire(Trigger.AbmaischTemperaturErreicht);
        }

        private void OnAbmaischenFertig()
        {
            _tempSoll = 0;
            Logger.Info("Abmaischen fertig!");
        }

        public string ToDotGraph()
        {
            return UmlDotGraph.Format(_machine.GetInfo());
        }
    }
}