using System;
using Stateless;
using Stateless.Graph;

namespace Brauknecht
{
    public class Maischautomatik
    {
        private readonly Maischprogramm _maischprogramm;

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

        private enum State
        {
            Aus,
            EinmaischenAufheizen,
            RastAufheizen,
            RastWarten,
            RastFertig,
            AbmaischenAufheizen
        }

        private State _state = State.Aus;
        readonly StateMachine<State, Trigger> _machine;

        private double _tempSoll;
        private int _dauerSoll;
        private int _index;

        public Maischautomatik(Maischprogramm maischprogramm)
        {
            _maischprogramm = maischprogramm;

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _machine.Configure(State.Aus)
                .Permit(Trigger.EinmaischenStart, State.EinmaischenAufheizen);

            _machine.Configure(State.EinmaischenAufheizen)
                .OnEntry(OnEinmaischenAufheizen)
                .OnExit(OnEinmaischenFertig)
                .Permit(Trigger.EinmaischenTemperaturErreicht, State.RastAufheizen);

            _machine.Configure(State.RastAufheizen)
                .OnEntry(() => OnRast(_index++))
                .Ignore(Trigger.RastAufheizenStart)
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

            // _machine.OnTransitioned(t =>
            //     Console.WriteLine(
            //         $"OnTransitioned: {t.Source} -> {t.Destination} via {t.Trigger}({string.Join(", ", t.Parameters)})"));
        }

        // Einmaischen
        public void Einmaischen()
        {
            _machine.Fire(Trigger.EinmaischenStart);
        }

        private void OnEinmaischenAufheizen()
        {
            _tempSoll = _maischprogramm.EinmaischTemperatur;
            Console.WriteLine($"Setze Einmaischen auf {_tempSoll}°C");
        }

        public void EinmaischenTemperaturErreicht()
        {
            _machine.Fire(Trigger.EinmaischenTemperaturErreicht);
        }

        private void OnEinmaischenFertig()
        {
            Console.WriteLine("Eingemaischt!");
        }


        // Rasten
        public void Rasten()
        {
            _machine.Fire(Trigger.RastAufheizenStart);
        }

        private void OnRast(int index)
        {
            _tempSoll = _maischprogramm.Rasten[index].Temperatur;
            _dauerSoll = _maischprogramm.Rasten[index].Dauer;
            Console.WriteLine($"Definiere Rast: {_tempSoll}°C, {_dauerSoll} min");
        }

        public void RastTemperaturErreicht()
        {
            _machine.Fire(Trigger.RastTemperaturErreicht);
        }

        private void OnRastWarten()
        {
            Console.WriteLine($"Rasttemperatur erreicht. Nun wird {_dauerSoll} min gewartet...");
        }

        public void RastWartenErreicht()
        {
            _machine.Fire(Trigger.RastWartenErreicht);
        }

        private void OnRastFertig()
        {
            Console.WriteLine("Rastdauer erreicht. Rast fertig!");
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
            _tempSoll = _maischprogramm.Abmaischtemperatur;
            Console.WriteLine($"Setze Abmaischen auf {_tempSoll}°C");
        }

        public void AbmaischTemperaturErreicht()
        {
            _machine.Fire(Trigger.AbmaischTemperaturErreicht);
        }

        private void OnAbmaischenFertig()
        {
            Console.WriteLine("Abmaischen fertig!");
        }

        public string ToDotGraph()
        {
            return UmlDotGraph.Format(_machine.GetInfo());
        }
    }
}