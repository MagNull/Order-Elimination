using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityExecution
    {
        public AbilityInstruction[] ActionInstructions; 

        public ActiveAbilityExecution(AbilityInstruction[] actionInstructions)
        {
            if (actionInstructions == null || actionInstructions.Any(i => i == null))
                Logging.LogException(new ArgumentNullException());
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
}
