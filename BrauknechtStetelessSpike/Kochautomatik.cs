using System;
using Stateless;
using Stateless.Graph;

namespace Brauknecht
{
    public class Kochautomatik
    {
        private readonly Kochprogramm _kochprogramm;

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
        private int _index;

        public Kochautomatik(Kochprogramm kochprogramm)
        {
            _kochprogramm = kochprogramm;

            Console.WriteLine($"Kochdauer {_kochprogramm.Kochdauer} min");

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _machine.Configure(State.Aus)
                .OnEntryFrom(Trigger.KochenBeendet, OnKochenBeendet)
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
                .OnEntry(() => OnHopfengabe(_index))
                .OnExit(() => OnHopfengabeExit(_index++))
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
            Console.WriteLine($"Kochtemperatur erreicht! {_kochprogramm.Kochdauer} min ab jetzt...");
        }

        public void Hopfengabe()
        {
            _machine.Fire(Trigger.HopfengabeStart);
        }

        private void OnHopfengabe(int index)
        {
            var dauer = _kochprogramm.Hopfengaben[index].Kochdauer;
            Console.WriteLine($"Hopfengabe bei {_kochprogramm.Kochdauer - dauer} min...");
        }

        public void HopengabeErreicht()
        {
            _machine.Fire(Trigger.HopfengabeErreicht);
        }

        private void OnHopfengabeExit(int index)
        {
            Console.WriteLine($"Hopfen ({_kochprogramm.Hopfengaben[index].Name}) rein!");
        }

        public void KochEndeErreicht()
        {
            _machine.Fire(Trigger.KochenBeendet);
        }

        private void OnKochenBeendet()
        {
            Console.WriteLine("Kochen beendet");
        }

        public string ToDotGraph()
        {
            return UmlDotGraph.Format(_machine.GetInfo());
        }
    }
}