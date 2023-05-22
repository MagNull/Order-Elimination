using OrderElimination.AbilitySystem.Animations;

namespace OrderElimination.AbilitySystem
{
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
