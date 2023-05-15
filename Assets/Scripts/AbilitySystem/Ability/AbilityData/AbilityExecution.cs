using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace OrderElimination.AbilitySystem
{
    public class AbilityExecution
    {
        public AbilityInstruction[] ActionInstructions;

        public AbilityExecution(AbilityInstruction[] actionInstructions)
        {
            if (actionInstructions == null)
                throw new ArgumentNullException();
            ActionInstructions = actionInstructions;
        }

        //TODO? return AbilityExecutionResult
        public async UniTask Execute(AbilityExecutionContext abilityExecutionContext)
        {
            foreach (var instruction in ActionInstructions)
            {
                await instruction.ExecuteRecursive(abilityExecutionContext);
            }
        }

        //public void AddInstructionsAfter<TAction>(ActionInstruction instructionToAdd, bool copyParentTargetGroups) where TAction : BattleAction<TAction>
        //{
        //    foreach (var instruction in ActionInstructions)
        //        instruction.AddInstructionsAfterRecursive<TAction>(instructionToAdd, copyParentTargetGroups);
        //}

        //public void RemoveInstructions(ActionInstruction instructionToRemove)
        //{
        //    foreach (var instruction in ActionInstructions)
        //        instruction.RemoveInstructionsRecursive(instructionToRemove);
        //}
    }

    //TODO Split AbilityExecutionContext and AbilityInstructionExecutionContext
    //Add Ability into InstructionContext
    public readonly struct AbilityExecutionContext
    {
        public readonly IBattleContext BattleContext;
        public readonly AbilitySystemActor AbilityCaster;
        public readonly CellGroupsContainer TargetedCellGroups;
        public readonly AnimationSceneContext AnimationSceneContext;
        public readonly AbilitySystemActor PreviousInstructionTarget;

        public AbilityExecutionContext(
            IBattleContext battleContext, 
            AbilitySystemActor abilityCaster, 
            CellGroupsContainer targetedCellGroups, 
            AbilitySystemActor previousInstructionTarget = null)
        {
            AnimationSceneContext = battleContext.AnimationSceneContext;
            BattleContext = battleContext;
            AbilityCaster = abilityCaster;
            TargetedCellGroups = targetedCellGroups;
            PreviousInstructionTarget = previousInstructionTarget;
        }

        public AbilityExecutionContext(
            AbilityExecutionContext basedContext,
            AbilitySystemActor previousInstructionTarget)
        {
            AnimationSceneContext = basedContext.AnimationSceneContext;
            BattleContext = basedContext.BattleContext;
            AbilityCaster = basedContext.AbilityCaster;
            TargetedCellGroups = basedContext.TargetedCellGroups;
            PreviousInstructionTarget = previousInstructionTarget;
        }
    }
}