using System;
using Stateless;
using Stateless.Graph;

namespace Brauknecht
{
    public class Maischautomatik
    {
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
            EinmaischenFertig,
            RastAufheizen,
            RastWarten,
            RastFertig,
            AbmaischenAufheizen,
            AbmaischenFertig
        }

        private State _state = State.Aus;
        readonly StateMachine<State, Trigger> _machine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<double> _setEinmaischenTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<double, int> _setRastTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<double> _setAbmaischenTrigger;

        private double _tempSoll;
        private int _dauerSoll;

        public Maischautomatik()
        {
            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _setEinmaischenTrigger = _machine.SetTriggerParameters<double>(Trigger.EinmaischenStart);
            _setRastTrigger = _machine.SetTriggerParameters<double, int>(Trigger.RastAufheizenStart);
            _setAbmaischenTrigger = _machine.SetTriggerParameters<double>(Trigger.AbmaischenStart);

            _machine.Configure(State.Aus)
                .Permit(Trigger.EinmaischenStart, State.EinmaischenAufheizen);

            _machine.Configure(State.EinmaischenAufheizen)
                .OnEntryFrom(_setEinmaischenTrigger, temp => OnEinmaischenAufheizen(temp),
                    "Aufheiztemperatur")
                .OnEntry(OnEinmaischenAufheizen)
                .Permit(Trigger.EinmaischenTemperaturErreicht, State.EinmaischenFertig);

            _machine.Configure(State.EinmaischenFertig)
                .OnEntry(OnEinmaischenFertig)
                .Permit(Trigger.RastAufheizenStart, State.RastAufheizen);

            _machine.Configure(State.RastAufheizen)
                .OnEntryFrom(_setRastTrigger, (temp, dauer) => OnRast(temp, dauer),
                    "Temperatur und Dauer der Rast")
                .OnEntry(OnRastAufheizen)
                .Permit(Trigger.RastTemperaturErreicht, State.RastWarten);

            _machine.Configure(State.RastWarten)
                .OnEntry(OnRastWarten)
                .Permit(Trigger.RastWartenErreicht, State.RastFertig);

            _machine.Configure(State.RastFertig)
                .OnEntry(OnRastFertig)
                .Permit(Trigger.RastAufheizenStart, State.RastAufheizen)
                .Permit(Trigger.AbmaischenStart, State.AbmaischenAufheizen);

            _machine.Configure(State.AbmaischenAufheizen)
                .OnEntryFrom(_setAbmaischenTrigger, temp => OnAbmaischenAufheizen(temp),
                    "Abmaischtemperatur")
                .OnEntry(OnAbmaischenAufheizen)
                .Permit(Trigger.AbmaischTemperaturErreicht, State.AbmaischenFertig);

            _machine.Configure(State.AbmaischenFertig)
                .OnEntry(OnAbmaischenFertig)
                .Permit(Trigger.EinmaischenStart, State.EinmaischenAufheizen);

            // _machine.OnTransitioned(t =>
            //     Console.WriteLine(
            //         $"OnTransitioned: {t.Source} -> {t.Destination} via {t.Trigger}({string.Join(", ", t.Parameters)})"));
        }
        
        // Einmaischen
        public void Einmaischen(double temp)
        {
            _machine.Fire(_setEinmaischenTrigger, temp);
        }

        private void OnEinmaischenAufheizen(in double temp)
        {
            _tempSoll = temp;
            Console.WriteLine($"Setze Einmaischen auf {_tempSoll}°C");
        }

        private void OnEinmaischenAufheizen()
        {
            // ReSharper disable once StringLiteralTypo
            Console.WriteLine("Einmaisching...");
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
        public void Rasten(double temp, int dauer)
        {
            _machine.Fire(_setRastTrigger, temp, dauer);
        }

        private void OnRast(in double temp, in int dauer)
        {
            _tempSoll = temp;
            _dauerSoll = dauer;
            Console.WriteLine($"Definiere Rast: {_tempSoll}°C, {_dauerSoll} min");
        }

        private void OnRastAufheizen()
        {
            Console.WriteLine($"Rasttemperatur auf {_tempSoll}°C aufheizen...");
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

        public void Abmaischen(double temp = 78.0)
        {
            _machine.Fire(_setAbmaischenTrigger, temp);
        }

        private void OnAbmaischenAufheizen(in double temp)
        {
            _tempSoll = temp;
            Console.WriteLine($"Setze Abmaischen auf {_tempSoll}°C");
        }

        private void OnAbmaischenAufheizen()
        {
            // ReSharper disable once StringLiteralTypo
            Console.WriteLine("Abmaisching...");
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