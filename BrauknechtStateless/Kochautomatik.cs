using BrauknechtStateless.PrgData;
using Stateless;
using Stateless.Graph;

namespace BrauknechtStateless
{
    public class Kochautomatik
    {
        private readonly Kochprogramm _prg;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private enum Trigger
        {
            VorderwürzeGegeben,
            KochenStart,
            KochTemperaturErreicht,
            HopfengabeWartenStart,
            HopfengabeErreicht,
            KochenBeendet,
        }

        public enum State
        {
            Aus,
            VorderwürzeHopfung,
            KochenAufheizen,
            Kochen,
            HopfengabeWarten,
            HopfenGeben
        }

        private State _state = State.Aus;
        public State CurrentState => _state;

        private readonly StateMachine<State, Trigger> _machine;
        private int _index;

        public Kochautomatik(Kochprogramm prg)
        {
            _prg = prg;

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _machine.Configure(State.Aus)
                .OnEntryFrom(Trigger.KochenBeendet, OnKochenBeendet)
                .OnExit(OnStartKochautomatik)
                .Permit(Trigger.VorderwürzeGegeben, State.VorderwürzeHopfung)
                .Permit(Trigger.KochenStart, State.KochenAufheizen);

            _machine.Configure(State.VorderwürzeHopfung)
                .OnEntry(OnVorderwürzegabe)
                .Permit(Trigger.KochenStart, State.KochenAufheizen);

            _machine.Configure(State.KochenAufheizen)
                .OnEntry(OnKochenAufheizen)
                .OnExit(OnKochtemperaturErreicht)
                .Permit(Trigger.KochenBeendet, State.Aus)
                .Permit(Trigger.KochTemperaturErreicht, State.Kochen);

            _machine.Configure(State.Kochen)
                .OnEntry(OnKochen)
                .Permit(Trigger.KochenBeendet, State.Aus)
                .PermitReentry(Trigger.KochenStart)
                .Permit(Trigger.HopfengabeWartenStart, State.HopfengabeWarten);

            _machine.Configure(State.HopfengabeWarten)
                .OnEntry(() => OnHopfengabe(_index))
                .Permit(Trigger.HopfengabeErreicht, State.HopfenGeben);

            _machine.Configure(State.HopfenGeben)
                .OnExit(() => OnHopfengegeben(_index++))
                .Permit(Trigger.HopfengabeWartenStart, State.HopfengabeWarten)
                .Permit(Trigger.KochenStart, State.Kochen)
                .Permit(Trigger.KochenBeendet, State.Aus);

            _machine.OnTransitioned(t => Logger.Debug(
                $"OnTransitioned: {t.Source} -> {t.Destination} via {t.Trigger}({string.Join(", ", t.Parameters)})"));
        }

        private void OnStartKochautomatik()
        {
            Logger.Info($"Kochdauer {_prg.Kochdauer} min");
        }

        public void VorderwürzeHopfungGegeben()
        {
            _machine.Fire(Trigger.VorderwürzeGegeben);
        }

        private void OnVorderwürzegabe()
        {
            Logger.Info("Vorderwürzegabe rein!");
        }

        private void OnKochenAufheizen()
        {
            // ReSharper disable once StringLiteralTypo
            Logger.Info("Aufheizing...");
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
            Logger.Info("Kochen...");
        }

        private void OnKochtemperaturErreicht()
        {
            Logger.Info($"Kochtemperatur erreicht! {_prg.Kochdauer} min ab jetzt...");
        }

        public void Hopfengabe()
        {
            _machine.Fire(Trigger.HopfengabeWartenStart);
        }

        private void OnHopfengabe(int index)
        {
            var dauer = _prg.Hopfengaben[index].Kochdauer;
            Logger.Info($"Hopfengabe bei {_prg.Kochdauer - dauer} min...");
        }

        public void HopengabeErreicht()
        {
            _machine.Fire(Trigger.HopfengabeErreicht);
        }

        private void OnHopfengegeben(int index)
        {
            Logger.Info($"Hopfen ({_prg.Hopfengaben[index].Name}) rein!");
        }

        public void KochEndeErreicht()
        {
            _machine.Fire(Trigger.KochenBeendet);
        }

        private void OnKochenBeendet()
        {
            Logger.Info("Kochen beendet");
        }

        public string ToDotGraph()
        {
            return UmlDotGraph.Format(_machine.GetInfo());
        }
    }
}