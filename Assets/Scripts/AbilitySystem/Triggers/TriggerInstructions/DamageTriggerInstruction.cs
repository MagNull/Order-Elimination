using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class DamageTriggerInstruction : ITriggerInstruction
    {
        private enum ActionsTarget
        {
            DamageDealer,
            DamageTarget
        }

        private List<IUndoableActionPerformResult> _undoableResults = new();

        private bool HasUndoableActions 
            => _actionsOnTarget != null && _actionsOnTarget.Any(a => a is IUndoableBattleAction);
        private bool HasDamageActions
            => _actionsOnTarget != null && _actionsOnTarget.Any(a => a is InflictDamageAction);

        [BoxGroup("Trigger", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        private ITriggerWithFireInfo<DamageTriggerFireInfo> _triggerSetup;

        [BoxGroup("Conditions", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        private ICommonCondition[] _commonConditions = new ICommonCondition[0];

        [BoxGroup("Conditions")]
        [ShowInInspector, OdinSerialize]
        private IEntityCondition[] _damageTargetConditions = new IEntityCondition[0];

        [BoxGroup("Action", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        private ActionsTarget _actionTarget;

        [BoxGroup("Action")]
        [ValidateInput(
            "@!" + nameof(HasDamageActions), 
            "Nested damage actions can cause an infinite cycle!")]
        [ShowInInspector, OdinSerialize]
        private IBattleAction[] _actionsOnTarget = new IBattleAction[0];

        [BoxGroup("Action")]
        [EnableIf("@" + nameof(HasUndoableActions))]
        [ShowInInspector, OdinSerialize]
        private bool _undoOnTriggerDeactivation;

        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var instance = _triggerSetup switch
            {
                IContextTriggerSetup contextSetup => contextSetup.GetTrigger(battleContext, caster),
                IEntityTriggerSetup entitySetup => entitySetup.GetTrigger(battleContext, caster, caster),
                _ => throw new NotImplementedException(),
            };
            instance.Triggered += OnTriggered;
            instance.Deactivated += OnTriggerDeactivation;
            return instance;

            async void OnTriggered(ITriggerFireInfo fireInfo)
            {
                var damageInflictInfo = (DamageTriggerFireInfo)fireInfo;
                if (_commonConditions != null 
                    && !_commonConditions.AllMet(battleContext, caster)
                    || _damageTargetConditions != null
                    && !_damageTargetConditions.AllMet(battleContext, caster, damageInflictInfo.DamageInfo.DamageTarget))
                    return;
                var target = _actionTarget switch
                {
                    ActionsTarget.DamageDealer => damageInflictInfo.DamageInfo.DamageDealer,
                    ActionsTarget.DamageTarget => damageInflictInfo.DamageInfo.DamageTarget,
                    _ => throw new NotImplementedException(),
                };
                var context = new ActionContext(
                    battleContext, CellGroupsContainer.Empty, caster, target, ActionCallOrigin.PassiveAbility);
                //Check conditions
                foreach (var action in _actionsOnTarget)
                {
                    var result = await action.ModifiedPerform(context);
                    if (_undoOnTriggerDeactivation && action is IUndoableBattleAction undoableAction)
                    {
                        _undoableResults.Add((IUndoableActionPerformResult)result);
                    }
                }
            }

            void OnTriggerDeactivation(IBattleTrigger trigger)
            {
                instance.Deactivated -= OnTriggerDeactivation;
                instance.Triggered -= OnTriggered;
                if (_undoOnTriggerDeactivation)
                {
                    foreach (var result in _undoableResults)
                    {
                        result.ModifiedAction.Undo(result.PerformId);
                    }
                }
            }
        }
    }
}
