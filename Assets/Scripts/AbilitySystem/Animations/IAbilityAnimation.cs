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
        public bool IsFinished { get; }

        public event Action<IAbilityAnimation> Finished;

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
        public readonly IAbilitySystemActor Caster;
        public readonly IAbilitySystemActor Target;

        public AnimationPlayContext(
            AnimationSceneContext sceneContext, 
            Vector2Int? casterGamePosition, 
            Vector2Int? targetGamePosition,  
            IAbilitySystemActor caster, 
            IAbilitySystemActor target)
        {
            SceneContext = sceneContext;
            CasterGamePosition = casterGamePosition;
            TargetGamePosition = targetGamePosition;
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
        }
    }
}
