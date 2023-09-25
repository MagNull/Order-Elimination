using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class RoundTracker : IBattleTracker
    {
        private int _awaitedRound = 1;
        private IBattleContext _trackingContext;

        [ShowInInspector, OdinSerialize]
        public int AwaitedRound
        {
            get => _awaitedRound;
            set
            {
                if (value < 1) value = 1;
                _awaitedRound = value;
            }
        }
        //public BattleSide TriggerOnTurnOf { get; private set; }

        public bool IsConditionMet { get; private set; }

        public event Action<IBattleTracker> ConditionMet;
        public event Action<IBattleTracker> ConditionLost;

        public void StartTracking(IBattleContext battleContext)
        {
            StopTracking();
            IsConditionMet = false;
            _trackingContext = battleContext;
            _trackingContext.NewRoundBegan += OnNewRound;
        }

        private void OnNewRound(IBattleContext battleContext)
        {
            if (battleContext.CurrentRound >= AwaitedRound)
            {
                if (!IsConditionMet)
                    ConditionMet?.Invoke(this);
            }
            else if (IsConditionMet)
            {
                ConditionLost?.Invoke(this);
            }
        }

        public void StopTracking()
        {
            if (_trackingContext == null)
                return;
            _trackingContext.NewRoundBegan -= OnNewRound;
        }
    }
}
