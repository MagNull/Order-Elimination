using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityExecution
    {
        public interface IPassiveExecutionActivationInfo
        {
            public IBattleContext ActivationContext { get; }
            public AbilitySystemActor Caster { get; }

            public event Action<IPassiveExecutionActivationInfo> Triggered;
        }

        protected class PassiveExecutionActivationInfo : IPassiveExecutionActivationInfo
        {
            private readonly IBattleTrigger[] _activationTriggers;

            public IBattleContext ActivationContext { get; }
            public AbilitySystemActor Caster { get; }
            public bool HasBeenActivated { get; private set; } = false;
            public bool HasBeenDeactivated { get; private set; } = false;

            public event Action<IPassiveExecutionActivationInfo> Triggered;

            public PassiveExecutionActivationInfo(
                IBattleContext context,
                AbilitySystemActor caster,
                IEnumerable<IBattleTrigger> activationTriggers)
            {
                ActivationContext = context;
                Caster = caster;
                _activationTriggers = activationTriggers.ToArray();
                foreach (var trigger in _activationTriggers)
                {
                    trigger.Triggered += OnTriggered;
                }
            }

            public void Activate()
            {
                if (HasBeenActivated)
                    Logging.LogException(new InvalidOperationException("Has already been activated."));
                foreach (var trigger in _activationTriggers)
                {
                    trigger.Activate();
                }
                HasBeenActivated = true;
            }

            public void Deactivate()
            {
                if (HasBeenDeactivated)
                    Logging.LogException( new InvalidOperationException("Has already been deactivated."));
                foreach (var trigger in _activationTriggers)
                {
                    trigger.Deactivate();
                    trigger.Triggered -= OnTriggered;
                }
                HasBeenDeactivated = true;
            }

            private void OnTriggered(ITriggerFireInfo fireInfo)
            {
                Triggered?.Invoke(this);
            }
        }

        public ITriggerInstruction[] TriggerInstructions { get; private set; }

        public event Action<IPassiveExecutionActivationInfo> ExecutionTriggered;

        public PassiveAbilityExecution(ITriggerInstruction[] triggerInstructions)
        {
            TriggerInstructions = triggerInstructions;
        }

        public IPassiveExecutionActivationInfo Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var activationInfo = new PassiveExecutionActivationInfo(
                battleContext,
                caster,
                TriggerInstructions.Select(i => i.GetActivationTrigger(battleContext, caster)));
            activationInfo.Triggered += OnTriggered;
            activationInfo.Activate();
            return activationInfo;
        }

        public void Dectivate(IPassiveExecutionActivationInfo activationInfo)
        {
            if (activationInfo == null)
            {
                Logging.LogException( new ArgumentNullException());
                throw new ArgumentNullException();
            }

            if (activationInfo is not PassiveExecutionActivationInfo specificInfo)
            {
                Logging.LogException( new ArgumentException("Unknown activation info implementation"));
                throw new ArgumentNullException();
            }
            if (specificInfo.HasBeenDeactivated)
                Logging.LogException( new InvalidOperationException("This execution has already been deactivated."));
            specificInfo.Deactivate();
            specificInfo.Triggered -= OnTriggered;
        }

        private void OnTriggered(IPassiveExecutionActivationInfo activationInfo)
        {
            ExecutionTriggered?.Invoke(activationInfo);
        }
    }
}
