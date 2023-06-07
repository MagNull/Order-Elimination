using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TimerTrigger : IContextTriggerSetup
    {
        [HideInInspector, OdinSerialize]
        private int _roundInterval = 1;

        [ShowInInspector]
        public int RoundInterval
        {
            get => _roundInterval;
            set
            {
                if (value < 1) value = 1;
                _roundInterval = value;
            }
        }

        [ShowInInspector, OdinSerialize]
        public bool TriggerOnStart { get; private set; }

        public IBattleTrigger GetTrigger(IBattleContext battleContext)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
            instance.ActivationRequested += OnActivation;
            return instance;
        }

        private void OnActivation(ITriggerSetup.BattleTrigger trigger)
        {
            trigger.ActivationRequested -= OnActivation;
            var passedRounds = 0;
            var interval = RoundInterval;
            var activationSide = trigger.OperatingContext.ActiveSide;
            trigger.OperatingContext.NewTurnStarted += OnNewTurn;
            trigger.DeactivationRequested += OnDeactivation;
            if (TriggerOnStart)
                Trigger();

            void OnNewTurn(IBattleContext battleContext)
            {
                if (battleContext.ActiveSide != activationSide)
                    return;
                passedRounds++;
                if (passedRounds % interval == 0)
                    Trigger();
            }

            void Trigger()
            {
                trigger.FireTrigger(new TimerTriggerFireInfo(trigger, passedRounds));
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                trigger.DeactivationRequested -= OnDeactivation;
                trigger.OperatingContext.NewTurnStarted -= OnNewTurn;
            }
        }
    }

    public class TimerTriggerFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
        public int PassedIntervals { get; }

        public TimerTriggerFireInfo(IBattleTrigger trigger, int passedIntervals)
        {
            Trigger = trigger;
            PassedIntervals = passedIntervals;
        }
    }
}
