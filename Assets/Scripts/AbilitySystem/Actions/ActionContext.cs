using OrderElimination.AbilitySystem.Animations;

namespace OrderElimination.AbilitySystem
{
    public class ActionContext
    {
        public readonly IBattleContext BattleContext;
        public readonly ActionCallOrigin CalledFrom;
        public readonly CellGroupsContainer CellTargetGroups;
        public readonly AbilitySystemActor ActionMaker;
        public readonly AbilitySystemActor ActionTarget;
        //AbilityUseContext (+ initial caster position, initial target position)

        public readonly AnimationSceneContext AnimationSceneContext;
        //CalledAbility - способность, инициирующая действия
        //CalledEffect - эффект, инициирующий действие

        public ActionContext(
            IBattleContext battleContext,
            CellGroupsContainer cellTargetGroups,
            AbilitySystemActor actionMaker,
            AbilitySystemActor target,
            ActionCallOrigin calledFrom)
        {
            BattleContext = battleContext;
            CalledFrom = calledFrom;
            CellTargetGroups = cellTargetGroups;
            ActionMaker = actionMaker;
            ActionTarget = target;
            //if (targetPosition == null && target != null && battleContext.BattleMap.ContainsEntity(target))
            //    targetPosition = target.Position;
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }
    }
}
