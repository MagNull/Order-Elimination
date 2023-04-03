using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilityExecution
    {
        // Действия по зонам(паттерну) способности
        //Изменить на список Группа-Действия ?
        public ActionInstruction[] ActionInstructions;

        //TODO? return AbilityExecutionResult
        public void Execute(AbilityExecutionContext abilityExecutionContext)
        {
            foreach (var instruction in ActionInstructions)
            {
                instruction.ExecuteRecursive(abilityExecutionContext);
            }
        }

        public void AddInstructionsAfter<TAction>(ActionInstruction instructionToAdd, bool copyParentTargetGroups) where TAction : BattleAction<TAction>
        {
            foreach (var instruction in ActionInstructions)
                instruction.AddInstructionsAfterRecursive<TAction>(instructionToAdd, copyParentTargetGroups);
        }

        public void RemoveInstructions(ActionInstruction instructionToRemove)
        {
            foreach (var instruction in ActionInstructions)
                instruction.RemoveInstructionsRecursive(instructionToRemove);
        }
    }

    public class AbilityExecutionContext
    {
        public readonly IBattleContext BattleContext;
        public readonly IAbilitySystemActor AbilityCaster;
        public readonly CellGroupsContainer TargetedCellGroups;

        public AbilityExecutionContext(IBattleContext battleContext, IAbilitySystemActor abilityCaster, CellGroupsContainer targetedCellGroups)
        {
            BattleContext = battleContext;
            AbilityCaster = abilityCaster;
            TargetedCellGroups = targetedCellGroups;
        }
    }
}
