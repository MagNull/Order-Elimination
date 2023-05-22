using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface ITriggerSetup
    {
        protected class BattleTrigger : IBattleTrigger//IDisposable
        {
            #region IBattleTrigger
            public bool IsActive { get; private set; }

            public event Action<ITriggerFireInfo> Triggered;

            public bool Activate()
            {
                if (IsActive) return false;
                IsActive = true;
                //
                //OperatingSetup.OnActivation(this);
                Activated?.Invoke(this);
                return true;
            }

            public bool Deactivate()
            {
                if (!IsActive) return false;
                IsActive = false;
                //
                Deactivated?.Invoke(this);
                return true;

                //Dispose
            }
            #endregion

            public event Action<BattleTrigger> Activated;
            public event Action<BattleTrigger> Deactivated;

            public ITriggerSetup OperatingSetup { get; }
            public IBattleContext OperatingContext { get; private set; }
            //public AbilitySystemActor TrackingEntity { get; private set; }

            public BattleTrigger(ITriggerSetup setup, IBattleContext context)
            {
                OperatingSetup = setup;
                OperatingContext = context;
            }

            public void Trigger(ITriggerFireInfo triggerFiredInfo)
            {
                if (!IsActive)
                {
                    throw new InvalidOperationException("Trigger hasn't been activated yet.");
                }
                Triggered?.Invoke(triggerFiredInfo);
                //foreach (var handler in Triggered.GetInvocationList().Select(d => (Action<ITriggerFireInfo>)d))
                //{
                //    await handler.Invoke(triggerFiredInfo);
                //}
            }

        }
    }

    public interface IContextTriggerSetup : ITriggerSetup
    {
        public IBattleTrigger GetTrigger(IBattleContext battleContext);
    }

    public interface IEntityTriggerSetup : ITriggerSetup
    {
        public abstract IBattleTrigger GetTrigger(
            IBattleContext battleContext, AbilitySystemActor trackingEntity);//AbilitySystemActor caster, AbilitySystemActor target);
    }
}
