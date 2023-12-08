using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class SideAnnihilationTracker : IBattleTracker
    {
        private IBattleContext _trackingContext;

        [ShowInInspector, OdinSerialize]
        public BattleSide Side { get; private set; }

        [ShowInInspector, OdinSerialize]
        public EnumMask<EntityType> TrackingEntities { get; private set; } = EnumMask<EntityType>.Full;

        public bool IsConditionMet { get; private set; }

        public event Action<IBattleTracker> ConditionMet;
        public event Action<IBattleTracker> ConditionLost;

        public void StartTracking(IBattleContext battleContext)
        {
            StopTracking();
            IsConditionMet = false;
            _trackingContext = battleContext;
            _trackingContext.EntitiesBank.BankChanged += OnEntitiesBankChanged;
            OnEntitiesBankChanged(_trackingContext.EntitiesBank);
        }

        public void StopTracking()
        {
            if (_trackingContext == null)
                return;
            _trackingContext.EntitiesBank.BankChanged -= OnEntitiesBankChanged;
        }

        private void OnEntitiesBankChanged(IReadOnlyEntitiesBank bank)
        {
            var noEntities = bank
                .GetActiveEntities(Side)
                .Where(e => TrackingEntities[e.EntityType])
                .Count() == 0;
            if (!IsConditionMet && noEntities)//Condition wasnt met before but is now
            {
                IsConditionMet = true;
                ConditionMet?.Invoke(this);
            }
            else if (IsConditionMet && !noEntities)//Condition was met before but no longer
            {
                IsConditionMet = false;
                ConditionLost?.Invoke(this);
            }
        }
    }
}
