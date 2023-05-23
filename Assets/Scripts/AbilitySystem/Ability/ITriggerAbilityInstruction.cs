using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ITriggerAbilityInstruction
    {
        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster);
    }

    public class SimpleTriggerInstruction : ITriggerAbilityInstruction
    {
        [ValidateInput(
            "@!(" + nameof(TriggerSetup) + " is " + nameof(IEntityTriggerSetup) + ")", 
            "*Will track Caster's condition.")]
        [ShowInInspector, OdinSerialize]
        public ITriggerSetup TriggerSetup { get; private set; }

        [ShowInInspector, OdinSerialize]
        public CasterRelativePattern CellDistributionPattern { get; private set; }

        [ShowInInspector, OdinSerialize]
        public AbilityInstruction[] Instructions { get; private set; }

        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster)
        {
            IBattleTrigger trigger;
            if (TriggerSetup is IContextTriggerSetup contextSetup)
            {
                trigger = contextSetup.GetTrigger(battleContext);
            }
            else if (TriggerSetup is IEntityTriggerSetup entitySetup)
            {
                var trackingEntity = caster;
                trigger = entitySetup.GetTrigger(battleContext, trackingEntity);
            }
            else
                throw new NotImplementedException();
            trigger.Triggered += OnTriggered;
            return trigger;

            async void OnTriggered(ITriggerFireInfo fireInfo)
            {
                var borders = battleContext.BattleMap.CellRangeBorders;
                var cellGroups = CellDistributionPattern.GetAffectedCellGroups(borders, caster.Position);
                var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups);
                foreach (var i in Instructions)
                {
                    await i.ExecuteRecursive(executionContext);
                }
            }
        }
    }

    public class EntitiesInZoneTriggerInstruction : ITriggerAbilityInstruction
    {
        private class UndoableResultsContainer
        {
            public HashSet<IUndoableActionPerformResult> OnEnterUndoableResults = new();
            public HashSet<IUndoableActionPerformResult> OnExitUndoableResults = new();
        }

        #region OdinVisuals
        private bool _hasUndoableEnterActions => _actionsOnEnter.Any(a => a is IUndoableBattleAction);
        private bool _hasUndoableLeaveActions => _actionsOnExit.Any(a => a is IUndoableBattleAction);
        #endregion

        [ShowInInspector, OdinSerialize]
        public EntityRelativeZoneTrigger TriggerSetup { get; private set; }

        #region AbilityInstructions based functionality
        //[ValidateInput("@false", "Use " + nameof(AbilityInstruction.AffectPreviousTarget) + " checkbox.")]
        //[ShowInInspector, OdinSerialize]
        //public AbilityInstruction InstructionForEnteredEntities { get; private set; }

        //[ValidateInput("@false", "Use " + nameof(AbilityInstruction.AffectPreviousTarget) + " checkbox.")]
        //[ShowInInspector, OdinSerialize]
        //public AbilityInstruction InstructionForLeavedEntities { get; private set; }
        #endregion

        [ShowInInspector, OdinSerialize]
        private List<IBattleAction> _actionsOnEnter { get; set; } = new();

        [ShowIf(nameof(_hasUndoableEnterActions))]
        [ShowInInspector, OdinSerialize]
        private bool _undoOnLeave { get; set; }

        [ShowInInspector, OdinSerialize]
        private List<IBattleAction> _actionsOnExit { get; set; } = new();

        [ShowIf(nameof(_hasUndoableLeaveActions))]
        [ShowInInspector, OdinSerialize]
        private bool _undoOnEnter { get; set; }


        public IBattleTrigger GetActivationTrigger(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var undoables = new Dictionary<AbilitySystemActor, UndoableResultsContainer>();

            var instance = TriggerSetup.GetTrigger(battleContext, caster);
            instance.Triggered += OnTriggered;//Never unsubscribes, but once trigger deactivated - never called
            //instance.Activate(); //Requires manual activation now
            return instance;

            async void OnTriggered(ITriggerFireInfo fireInfo)
            {
                var triggerZoneInfo = (TriggerZoneFireInfo)fireInfo;
                await Execute(triggerZoneInfo, battleContext, caster, undoables);
            }
        }

        private async UniTask Execute(
            TriggerZoneFireInfo info, 
            IBattleContext battleContext, 
            AbilitySystemActor caster,
            Dictionary<AbilitySystemActor, UndoableResultsContainer> undoables)
        {
            var cellGroups = CellGroupsContainer.Empty;
            foreach (var leaver in info.DisappearedEntities)
            {
                //var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups, leaver);
                //InstructionForLeavedEntities.ExecuteRecursive(executionContext);
                var actionContext = new ActionContext(battleContext, cellGroups, caster, leaver);
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
            foreach (var newcomer in info.NewEntities)
            {
                //var executionContext = new AbilityExecutionContext(battleContext, caster, cellGroups, newcomer);
                //InstructionForEnteredEntities.ExecuteRecursive(executionContext);
                var actionContext = new ActionContext(battleContext, cellGroups, caster, newcomer);
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
