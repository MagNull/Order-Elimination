using Cysharp.Threading.Tasks;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityExecution
    {
        public AbilityInstruction[] ActionInstructions { get; } 

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

        public bool AppendInstructionRecursively(
            Predicate<AbilityInstruction> parentSelector,
            AbilityInstruction newInstruction,
            bool copyParentTargetGroups,
            InstructionFollowType followType)
        {
            foreach (var instruction in ActionInstructions)
            {
                if (!instruction.AppendInstructionRecursively(
                    parentSelector, newInstruction, copyParentTargetGroups, followType))
                    return false;
            }
            return true;
        }

        public bool ModifyInstructionRecursively(
            Predicate<AbilityInstruction> selector,
            Func<AbilityInstruction, AbilityInstruction> modifier)
        {
            foreach (var instruction in ActionInstructions)
            {
                if (!instruction.ModifyInstructionRecursively(selector, modifier))
                    return false;
            }
            return true;
        }
    }
}
