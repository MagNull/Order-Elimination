using OrderElimination.AbilitySystem.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public enum TriggerTrackingEntity
    {
        Caster,
        Target
    }

    public interface ITriggerSetup
    {
        protected class BattleTrigger : IBattleTrigger//IDisposable
        {
            public bool IsActive { get; private set; }

            public event Action<ITriggerFiredInfo> Triggered;
            public event Action<BattleTrigger> Deactivated;

            private ITriggerSetup _runningSetup;
            private TriggerActivationContext? _operatingContext;

            public void ForceTrigger(ITriggerFiredInfo triggerFiredInfo)
            {
                if (!IsActive)
                    return;//false
                Triggered?.Invoke(triggerFiredInfo);
            }

            public BattleTrigger(ITriggerSetup setup)
            {
                _runningSetup = setup;
            }
        }
    }

    public interface IContextTriggerSetup : ITriggerSetup
    {
        public IBattleTrigger GetTrigger(IBattleContext battleContext);
    }

    public interface IEntityTrackingTriggerSetup : ITriggerSetup
    {
        public abstract IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor trackingEntity);//AbilitySystemActor caster, AbilitySystemActor target);
    }
}
