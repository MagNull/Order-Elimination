using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationPlayContext
    {
        public readonly AnimationSceneContext SceneContext;
        public readonly CellGroupsContainer TargetedCellGroups;
        public readonly AbilitySystemActor Caster;//Remove, leave only position + view
        public readonly AbilitySystemActor Target;
        public readonly Vector2Int? CasterGamePosition;
        public readonly Vector2Int? TargetGamePosition;
        public readonly BattleEntityView CasterView;//Null if Caster is null
        public readonly BattleEntityView TargetView;//Null if Target is null
        public readonly Vector3? CasterVisualPosition;
        public readonly Vector3? TargetVisualPosition;

        public AnimationPlayContext(
            AnimationSceneContext sceneContext,
            CellGroupsContainer targetedCellGroups,
            AbilitySystemActor caster,
            AbilitySystemActor target,
            Vector2Int? targetPositionOverride = null)
        {
            SceneContext = sceneContext;
            TargetedCellGroups = targetedCellGroups;
            Caster = caster;
            Target = target;
            CasterView = null;
            TargetView = null;
            CasterVisualPosition = null;
            TargetVisualPosition = null;
            CasterGamePosition = null;
            TargetGamePosition = null;

            if (caster != null)
            {
                var pos = caster.BattleContext.BattleMap.GetLastPosition(caster);
                CasterGamePosition = pos;
                CasterVisualPosition = sceneContext.BattleMapView.GetCell(pos.x, pos.y).transform.position;
                CasterView = sceneContext.EntitiesBank.GetViewByEntity(caster);
            }
            if (target != null)
            {
                var pos = target.BattleContext.BattleMap.GetLastPosition(target);
                TargetGamePosition = pos;
                TargetView = sceneContext.EntitiesBank.GetViewByEntity(target);
                TargetVisualPosition = sceneContext.BattleMapView.GetCell(pos.x, pos.y).transform.position;
            }
            if (targetPositionOverride != null)
                TargetGamePosition = targetPositionOverride;
        }

        public static AnimationPlayContext Duplicate(
            AnimationPlayContext originalContext, 
            Vector2Int? targetPosition = null,
            AbilitySystemActor caster = null,
            AbilitySystemActor target = null)
        {
            return new AnimationPlayContext(
                originalContext.SceneContext,
                originalContext.TargetedCellGroups,
                caster ?? originalContext.Caster,
                target ?? originalContext.Target,
                targetPosition ?? originalContext.TargetGamePosition);
        }
    }
}
