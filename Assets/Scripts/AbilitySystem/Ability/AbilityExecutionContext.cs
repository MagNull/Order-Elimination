using OrderElimination.AbilitySystem.Animations;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //TODO Split AbilityExecutionContext and AbilityInstructionExecutionContext
    //Add Ability into InstructionContext
    public readonly struct AbilityExecutionContext
    {
        public readonly ActionCallOrigin CallOrigin;
        public readonly IBattleContext BattleContext;
        public readonly AnimationSceneContext AnimationSceneContext;
        public readonly AbilitySystemActor AbilityCaster;
        public readonly CellGroupsContainer CellTargetGroups;

        public readonly Vector2Int? SpecifiedCell;
        public readonly AbilitySystemActor SpecifiedEntity;

        public bool HasSpecifiedTarget => SpecifiedEntity != null || SpecifiedCell != null;

        public AbilityExecutionContext(
            ActionCallOrigin callOrigin,
            IBattleContext battleContext, 
            AbilitySystemActor abilityCaster, 
            CellGroupsContainer cellGroups)
        {
            CallOrigin = callOrigin;
            AnimationSceneContext = battleContext.AnimationSceneContext;
            BattleContext = battleContext;
            AbilityCaster = abilityCaster;
            CellTargetGroups = cellGroups;
            SpecifiedCell = null;
            SpecifiedEntity = null;
        }

        private AbilityExecutionContext(
            AbilityExecutionContext basedContext,
            Vector2Int specifiedCell)
        {
            AnimationSceneContext = basedContext.AnimationSceneContext;
            CallOrigin = basedContext.CallOrigin;
            BattleContext = basedContext.BattleContext;
            AbilityCaster = basedContext.AbilityCaster;
            CellTargetGroups = basedContext.CellTargetGroups;
            SpecifiedCell = specifiedCell;
            SpecifiedEntity = null;
        }

        private AbilityExecutionContext(
            AbilityExecutionContext basedContext,
            AbilitySystemActor specifiedEntity)
        {
            AnimationSceneContext = basedContext.AnimationSceneContext;
            CallOrigin = basedContext.CallOrigin;
            BattleContext = basedContext.BattleContext;
            AbilityCaster = basedContext.AbilityCaster;
            CellTargetGroups = basedContext.CellTargetGroups;
            SpecifiedCell = specifiedEntity != null
                ? basedContext.BattleContext.BattleMap.GetLastPosition(specifiedEntity)
                : null;
            SpecifiedEntity = specifiedEntity;
        }

        public AbilityExecutionContext SpecifyCell(Vector2Int specifiedCell)
            => new(this, specifiedCell);

        public AbilityExecutionContext SpecifyEntity(AbilitySystemActor specifiedEntity)
            => new(this, specifiedEntity);
    }
}
