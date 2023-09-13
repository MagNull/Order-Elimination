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
            public AbilitySystemActor Owner { get; }

            public event Action<IPassiveExecutionActivationInfo> Triggered;
        }

        protected class PassiveExecutionActivationInfo : IPassiveExecutionActivationInfo
        {
            private readonly IBattleAction[] _activationActions;
            private readonly IBattleTrigger[] _activationTriggers;
            private List<IUndoableActionPerformResult> _undoablePerforms = new();

            public IBattleContext ActivationContext { get; }
            public AbilitySystemActor Owner { get; }
            public bool HasBeenActivated { get; private set; } = false;
            public bool HasBeenDeactivated { get; private set; } = false;

            public event Action<IPassiveExecutionActivationInfo> Triggered;

            public PassiveExecutionActivationInfo(
                IBattleContext context,
                AbilitySystemActor owner,
                IEnumerable<IBattleAction> activationActions,
                IEnumerable<IBattleTrigger> activationTriggers)
            {
                ActivationContext = context;
                Owner = owner;
                _activationActions = activationActions.ToArray();
                _activationTriggers = activationTriggers.ToArray();
                foreach (var trigger in _activationTriggers)
                {
                    trigger.Triggered += OnTriggered;
                }
            }

            public async void Activate()
            {
                if (HasBeenActivated)
                    Logging.LogException(new InvalidOperationException("Has already been activated."));
                var actionContext = new ActionContext(ActivationContext, null, Owner, Owner);
                foreach (var action in _activationActions)
                {
                    //CallbackAction check
                    //UndoableAction check
                    var result = await action.ModifiedPerform(actionContext, false, false);
                    if (result is IUndoableActionPerformResult undoableActionResult)
                    {
                        _undoablePerforms.Add(undoableActionResult);
                    }
                }
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
                foreach (var undoableActionPerform in _undoablePerforms)
                {
                    undoableActionPerform.ModifiedAction.Undo(undoableActionPerform.PerformId);
                }
                _undoablePerforms.Clear();
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

        public IBattleAction[] ActionsOnActivation { get; private set; }//actions with owner
        public ITriggerInstruction[] TriggerInstructions { get; private set; }

        public event Action<IPassiveExecutionActivationInfo> ExecutionTriggered;

        public PassiveAbilityExecution(
            IBattleAction[] actionsOnActivation, ITriggerInstruction[] triggerInstructions)
        {
            ActionsOnActivation = actionsOnActivation.ToArray();
            TriggerInstructions = triggerInstructions.ToArray();
        }

        public IPassiveExecutionActivationInfo Activate(
            IBattleContext battleContext, AbilitySystemActor owner)
        {
            var activationInfo = new PassiveExecutionActivationInfo(
                battleContext,
                owner,
                ActionsOnActivation,
                TriggerInstructions.Select(i => i.GetActivationTrigger(battleContext, owner)));
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
