using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ITriggerAbilityInstruction
    {
        public IBattleTrigger Activate(IBattleContext battleContext, AbilitySystemActor caster);
        //Deactivate?
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


        public IBattleTrigger Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            var undoables = new Dictionary<AbilitySystemActor, UndoableResultsContainer>();

            var instance = TriggerSetup.GetTrigger(battleContext, caster);
            instance.Triggered += OnTriggered;//Never unsubscribes, but once trigger deactivated - never called
            instance.Activate();
            return instance;

            void OnTriggered(ITriggerFireInfo fireInfo)
            {
                var triggerZoneInfo = (TriggerZoneFireInfo)fireInfo;
                Execute(triggerZoneInfo, battleContext, caster, undoables);
            }
        }

        private async void Execute(
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
