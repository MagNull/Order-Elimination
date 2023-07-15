using AI.Conditions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class EntitiesInZoneTriggerInstruction : ITriggerInstruction
    {
        private enum ActionsTarget
        {
            EnteredEntity,
            Caster
        }

        private class UndoableResultsContainer
        {
            public HashSet<IUndoableActionPerformResult> OnEnterUndoableResults = new();
            public HashSet<IUndoableActionPerformResult> OnExitUndoableResults = new();
        }

        #region OdinVisuals
        private bool _hasUndoableEnterActions => _actionsOnEnter.Any(a => a is IUndoableBattleAction);
        private bool _hasUndoableExitActions => _actionsOnExit.Any(a => a is IUndoableBattleAction);
        #endregion

        [ShowInInspector, OdinSerialize]
        public EntityRelativeZoneTrigger TriggerSetup { get; private set; } = new();

        #region AbilityInstructions based functionality
        //[ValidateInput("@false", "Use " + nameof(AbilityInstruction.AffectPreviousTarget) + " checkbox.")]
        //[ShowInInspector, OdinSerialize]
        //public AbilityInstruction InstructionForEnteredEntities { get; private set; }

        //[ValidateInput("@false", "Use " + nameof(AbilityInstruction.AffectPreviousTarget) + " checkbox.")]
        //[ShowInInspector, OdinSerialize]
        //public AbilityInstruction InstructionForLeavedEntities { get; private set; }
        #endregion

        [ShowIf("@" + nameof(_hasUndoableEnterActions) + " || " + nameof(_hasUndoableExitActions))]
        [ShowInInspector, OdinSerialize]
        private bool _undoOnTriggerDeactivation { get; set; }

        [ShowInInspector, OdinSerialize]
        private ActionsTarget _actionTarget { get; set; }

        //[ShowInInspector, OdinSerialize]
        //private List<ITargetCondition> _targetConditions { get; set; } = new ();

        [ShowInInspector, OdinSerialize]
        private List<IBattleAction> _actionsOnEnter { get; set; } = new();

        [ShowIf(nameof(_hasUndoableEnterActions))]
        [ValidateInput(
            "@" + nameof(TriggerSetup) + "." + nameof(EntityRelativeZoneTrigger.TriggerOnExit),
            "Enable " + nameof(EntityRelativeZoneTrigger.TriggerOnExit) + " in trigger setup for undo to work!")]
        [ShowInInspector, OdinSerialize]
        private bool _undoOnLeave { get; set; }

        [ShowInInspector, OdinSerialize]
        private List<IBattleAction> _actionsOnExit { get; set; } = new();

        [ShowIf(nameof(_hasUndoableExitActions))]
        [ValidateInput(
            "@" + nameof(TriggerSetup) + "." + nameof(EntityRelativeZoneTrigger.TriggerOnEnter),
            "Enable " + nameof(EntityRelativeZoneTrigger.TriggerOnEnter) + " in trigger setup for undo to work!")]
        [ShowInInspector, OdinSerialize]
        private bool _undoOnEnter { get; set; }

        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var undoables = new Dictionary<AbilitySystemActor, UndoableResultsContainer>();
            AbilitySystemActor[] entitiesInZone = null;

            var instance = TriggerSetup.GetTrigger(battleContext, caster);
            instance.Triggered += OnTriggered;
            instance.Deactivated += OnTriggerDeactivation;
            return instance;

            async void OnTriggered(ITriggerFireInfo fireInfo)
            {
                var triggerZoneInfo = (ZoneTriggerFireInfo)fireInfo;
                entitiesInZone = triggerZoneInfo.CurrentEntities;
                await Execute(
                    triggerZoneInfo.DisappearedEntities, 
                    triggerZoneInfo.NewEntities, 
                    battleContext, 
                    caster, 
                    undoables);
            }

            async void OnTriggerDeactivation(IBattleTrigger trigger)
            {
                instance.Deactivated -= OnTriggerDeactivation;
                instance.Triggered -= OnTriggered;
                if (_undoOnTriggerDeactivation && entitiesInZone != null)
                {
                    foreach (var entity in entitiesInZone.Where(e => undoables.ContainsKey(e)))
                    {
                        foreach (var enterResult in undoables[entity].OnEnterUndoableResults)
                        {
                            enterResult.ModifiedAction.Undo(enterResult.PerformId);
                        }
                        foreach (var enterResult in undoables[entity].OnExitUndoableResults)
                        {
                            enterResult.ModifiedAction.Undo(enterResult.PerformId);
                        }
                    }
                    
                }
            }
        }

        private async UniTask Execute(
            AbilitySystemActor[] disappearedEntities,
            AbilitySystemActor[] newEntities,
            IBattleContext battleContext, 
            AbilitySystemActor caster,
            Dictionary<AbilitySystemActor, UndoableResultsContainer> undoables)
        {
            var cellGroups = CellGroupsContainer.Empty;
            foreach (var leaver in disappearedEntities)
            {
                //var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups, leaver);
                //InstructionForLeavedEntities.ExecuteRecursive(executionContext);

                var targetEntity = _actionTarget switch
                {
                    ActionsTarget.EnteredEntity => leaver,
                    ActionsTarget.Caster => caster,
                    _ => throw new System.NotImplementedException(),
                };
                var actionContext = new ActionContext(battleContext, cellGroups, caster, targetEntity);
                foreach (var action in _actionsOnExit)
                {
                    var result = await action.ModifiedPerform(actionContext);
                    if (result is IUndoableActionPerformResult undoableResult)
                    {
                        if (!undoables.ContainsKey(leaver))
                            undoables.Add(leaver, new UndoableResultsContainer());
                        undoables[leaver].OnExitUndoableResults.Add(undoableResult);
                    }
                }
                if (_undoOnLeave && undoables.ContainsKey(leaver))
                {
                    foreach (var enterResult in undoables[leaver].OnEnterUndoableResults)
                    {
                        enterResult.ModifiedAction.Undo(enterResult.PerformId);
                    }
                }
            }
            foreach (var newcomer in newEntities)
            {
                //var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups, newcomer);
                //InstructionForEnteredEntities.ExecuteRecursive(executionContext);
                var targetEntity = _actionTarget switch
                {
                    ActionsTarget.EnteredEntity => newcomer,
                    ActionsTarget.Caster => caster,
                    _ => throw new System.NotImplementedException(),
                };
                var actionContext = new ActionContext(battleContext, cellGroups, caster, targetEntity);
                foreach (var action in _actionsOnEnter)
                {
                    var result = await action.ModifiedPerform(actionContext);
                    if (result is IUndoableActionPerformResult undoableResult)
                    {
                        if (!undoables.ContainsKey(newcomer))
                            undoables.Add(newcomer, new UndoableResultsContainer());
                        undoables[newcomer].OnEnterUndoableResults.Add(undoableResult);
                    }
                }
                if (_undoOnEnter && undoables.ContainsKey(newcomer))
                {
                    foreach (var exitResult in undoables[newcomer].OnExitUndoableResults)
                    {
                        exitResult.ModifiedAction.Undo(exitResult.PerformId);
                    }
                }
            }
        }
    }
}
