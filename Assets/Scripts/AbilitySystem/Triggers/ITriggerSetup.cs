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
            private bool _hasBeenActivated = false;

            #region IBattleTrigger
            public bool IsActive { get; private set; }

            public event Action<ITriggerFireInfo> Triggered;
            public event Action<IBattleTrigger> Deactivated;
            //public event Action<IBattleTrigger> AllTriggerHandlersExecuted;

            public bool Activate()
            {
                if (IsActive) return false;
                IsActive = true;
                //
                //OperatingSetup.OnActivation(this);
                ActivationRequested?.Invoke(this);
                _hasBeenActivated = true;
                return true;
            }

            public bool Deactivate()
            {
                if (!IsActive) return false;
                IsActive = false;
                //
                DeactivationRequested?.Invoke(this);
                if (Triggered != null)
                {
                    foreach (var handler in Triggered.GetInvocationList().Cast<Action<ITriggerFireInfo>>())
                    {
                        Triggered -= handler;
                    }
                }
                Deactivated?.Invoke(this);
                return true;

                //Dispose
            }
            #endregion

            public event Action<BattleTrigger> ActivationRequested;
            public event Action<BattleTrigger> DeactivationRequested;

            public ITriggerSetup OperatingSetup { get; }
            public IBattleContext OperatingContext { get; private set; }
            //public AbilitySystemActor TrackingEntity { get; private set; }

            public BattleTrigger(ITriggerSetup setup, IBattleContext context)
            {
                OperatingSetup = setup;
                OperatingContext = context;
            }

            public void FireTrigger(ITriggerFireInfo triggerFiredInfo)
            {
                if (!IsActive)
                {
                    //TODO: Fix and remove "return". It shouldn't even call Trigger() after instance diactivation.
                    return;
                    Logging.LogException( new InvalidOperationException("Trigger hasn't been activated yet or has already been deactivated."));
                }
                Triggered?.Invoke(triggerFiredInfo);
                //AllTriggerHandlersExecuted?.Invoke(this);
                //foreach (var handler in Triggered.GetInvocationList().Select(d => (Func<ITriggerFireInfo, UniTask>)d))
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
