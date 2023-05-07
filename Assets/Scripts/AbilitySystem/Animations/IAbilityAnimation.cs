using Assets.AbilitySystem.PrototypeHelpers;
using Cysharp.Threading.Tasks;
using System;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using VContainer;

namespace OrderElimination.AbilitySystem.Animations
{
    public interface IAbilityAnimation
    {
        public UniTask Play(AnimationPlayContext context);
    }

    public struct AnimationPlayContext
    {
        public readonly AnimationSceneContext SceneContext;
        public readonly Vector2Int? CasterGamePosition;
        public readonly Vector2Int? TargetGamePosition;
        public readonly Vector2? CasterVisualPosition;
        public readonly Vector2? TargetVisualPosition;
        public readonly BattleEntityView CasterView;//Null if Caster is null
        public readonly BattleEntityView TargetView;//Null if Target is null
        public readonly AbilitySystemActor Caster;
        public readonly AbilitySystemActor Target;
        public readonly CellGroupsContainer TargetedCellGroups;

        public AnimationPlayContext(
            AnimationSceneContext sceneContext,
            CellGroupsContainer targetedCellGroups,
            Vector2Int? casterPosition,
            Vector2Int? targetPosition,
            AbilitySystemActor caster,
            AbilitySystemActor target)
        {
            SceneContext = sceneContext;
            CasterGamePosition = casterPosition;
            TargetGamePosition = targetPosition;
            CasterVisualPosition = null;
            TargetVisualPosition = null;
            if (CasterGamePosition != null && CasterGamePosition.HasValue)
            {
                var pos = CasterGamePosition.Value;
                CasterVisualPosition = sceneContext.BattleMapView.GetCell(pos.x, pos.y).transform.position;
            }
            if (TargetGamePosition != null && TargetGamePosition.HasValue)
            {
                var pos = TargetGamePosition.Value;
                TargetVisualPosition = sceneContext.BattleMapView.GetCell(pos.x, pos.y).transform.position;
            }
            Caster = caster;
            Target = target;
            CasterView = null;
            TargetView = null;
            if (caster != null)
                CasterView = sceneContext.EntitiesBank.GetViewByEntity(caster);
            if (target != null)
                TargetView = sceneContext.EntitiesBank.GetViewByEntity(target);
            TargetedCellGroups = targetedCellGroups;
        }

        public static AnimationPlayContext Duplicate(
            AnimationPlayContext originalContext, 
            Vector2Int? casterPosition = null,
            Vector2Int? targetPosition = null,
            AbilitySystemActor caster = null,
            AbilitySystemActor target = null)
        {
            return new AnimationPlayContext(
                originalContext.SceneContext,
                originalContext.TargetedCellGroups,
                casterPosition ?? originalContext.CasterGamePosition,
                targetPosition ?? originalContext.TargetGamePosition,
                caster ?? originalContext.Caster,
                target ?? originalContext.Target);
        }
    }
}
