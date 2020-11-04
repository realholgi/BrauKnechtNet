using System;
using Stateless;
using Stateless.Graph;

namespace Brauknecht
{
    public class Kochautomatik
    {
        private enum Trigger
        {
            VorderwürzeGegeben,
            KochenStart,
            KochTemperaturErreicht,
            HopfengabeStart,
            HopfengabeErreicht,
            KochenBeendet,
        }

        private enum State
        {
            Aus,
            Vorderwürze,
            KochenAufheizen,
            Kochen,
            Hopfengabe,
        }

        private State _state = State.Aus;
        private readonly StateMachine<State, Trigger> _machine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<int> _setHopfengabeTrigger;
        
        private readonly int _kochdauer;

        public Kochautomatik(int dauer)
        {
            _kochdauer = dauer;

            Console.WriteLine($"Kochdauer {_kochdauer} min");

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _setHopfengabeTrigger = _machine.SetTriggerParameters<int>(Trigger.HopfengabeStart);

            _machine.Configure(State.Aus)
                .Permit(Trigger.VorderwürzeGegeben, State.Vorderwürze)
                .Permit(Trigger.KochenStart, State.KochenAufheizen);

            _machine.Configure(State.Vorderwürze)
                .OnEntry(OnVorderwürzegabe)
                .Permit(Trigger.KochenStart, State.KochenAufheizen);

            _machine.Configure(State.KochenAufheizen)
                .OnEntry(OnKochenAufheizen)
                .OnExit(OnKochtemperaturErreicht)
                .Permit(Trigger.KochTemperaturErreicht, State.Kochen);

            _machine.Configure(State.Kochen)
                .OnEntry(OnKochen)
                .Permit(Trigger.KochenBeendet, State.Aus)
                .Permit(Trigger.HopfengabeStart, State.Hopfengabe);

            _machine.Configure(State.Hopfengabe)
                .OnEntryFrom(_setHopfengabeTrigger, OnHopfengabe, "Kochdauer Hopfengabe")
                .OnExit(OnHopfengabeExit)
                .Permit(Trigger.HopfengabeErreicht, State.Kochen)
                .Permit(Trigger.KochenBeendet, State.Aus);

            // _machine.OnTransitioned(t =>
            //     Console.WriteLine(
            //         $"OnTransitioned: {t.Source} -> {t.Destination} via {t.Trigger}({string.Join(", ", t.Parameters)})"));
        }

        public void VorderwürzeGegeben()
        {
            _machine.Fire(Trigger.VorderwürzeGegeben);
        }

        private void OnVorderwürzegabe()
        {
            Console.WriteLine("Vorderwürzegabe rein!");
        }

        private void OnKochenAufheizen()
        {
            // ReSharper disable once StringLiteralTypo
            Console.WriteLine("Aufheizing...");
        }

        public void KochTemperaturErreicht()
        {
            _machine.Fire(Trigger.KochTemperaturErreicht);
        }

        public void Kochen()
        {
            _machine.Fire(Trigger.KochenStart);
        }

        private void OnKochen()
        {
            Console.WriteLine("Kochen...");
        }

        private void OnKochtemperaturErreicht()
        {
            Console.WriteLine($"Kochtemperatur erreicht! {_kochdauer} min ab jetzt...");
        }

        public void Hopfengabe(int kochdauer)
        {
            _machine.Fire(_setHopfengabeTrigger, kochdauer);
        }

        private void OnHopfengabe(int dauer)
        {
            Console.WriteLine($"Hopfengabe bei {_kochdauer - dauer} min...");
        }

        public void HopengabeErreicht()
        {
            _machine.Fire(Trigger.HopfengabeErreicht);
        }

        private void OnHopfengabeExit()
        {
            Console.WriteLine("Hopfengabe!");
        }

        public void KochEndeErreicht()
        {
            _machine.Fire(Trigger.KochenBeendet);
        }
        
        public string ToDotGraph()
        {
            return UmlDotGraph.Format(_machine.GetInfo());
        }
    }
}