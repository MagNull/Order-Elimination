using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class EffectTriggerAcceptor
    {
        //private bool _isEntityTrigger;
        private ITriggerSetup _triggerSetup;

        [ShowInInspector, OdinSerialize]
        public ITriggerSetup TriggerSetup
        {
            get => _triggerSetup;
            private set
            {
                IsSetupEntityTracking = false;
                if (value is IEntityTriggerSetup)
                    IsSetupEntityTracking = true;
                _triggerSetup = value;
            }
        }

        [ShowIf("@" + nameof(IsSetupEntityTracking))]
        [ShowInInspector, OdinSerialize]
        public EffectEntity TrackingEntity { get; private set; }

        [ShowInInspector, DisplayAsString]
        public bool IsSetupEntityTracking { get; private set; }

        public IBattleTrigger GetTrigger(
            IBattleContext battleContext, AbilitySystemActor effectHolder, AbilitySystemActor effectApplier)
        {
            if (TriggerSetup is IContextTriggerSetup contextSetup)
            {
                return contextSetup.GetTrigger(battleContext);
            }
            if (TriggerSetup is IEntityTriggerSetup entitySetup)
            {
                var trackingEntity = TrackingEntity switch
                {
                    EffectEntity.EffectHolder => effectHolder,
                    EffectEntity.EffectApplier => effectApplier,
                    _ => throw new NotImplementedException(),
                };
                return entitySetup.GetTrigger(battleContext, trackingEntity);
            }
            Logging.LogException( new NotImplementedException());
            throw new NotImplementedException();
        }
    }

}
