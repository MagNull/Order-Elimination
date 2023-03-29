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

        public bool StartCast(IBattleContext battleContext, AbilityExecutionContext useContext)
        {
            //Start
            //1. Show available cells
            //2. Player selects target cell from available
            //3. Ability shows affected cells by CellGroups (Main, Area, Secondary, ...)

            //Player Confirms usage

            //Execution
            // Process ApplyingActions on Caster
            // Process ApplyingActions on Target

            //Usage
            //Permorm Actions
            // Run Triggers after usage
            return true;
        }

        //TODO return AbilityExecutionResult
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
                instruction.AddInstructionsAfter<TAction>(instructionToAdd, copyParentTargetGroups);
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
