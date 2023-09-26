using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class SpawnParticleAnimation : AwaitableAbilityAnimation
    {
        [HideInInspector, OdinSerialize]
        private int _animationLoops = 1;

        [ShowInInspector, OdinSerialize] 
        public ParticleType ParticleType { get; private set; }

        [TabGroup("Location")]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget SpawnAt { get; private set; }

        [TabGroup("Location")]
        [ShowIf("@" + nameof(SpawnAt) + " == AnimationTarget.CellGroup")]
        [ShowInInspector, OdinSerialize]
        public int SpawnGroup { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ParticleFollowOption Follow { get; private set; } = ParticleFollowOption.DontFollow;

        [TabGroup("Direction")]
        [ShowInInspector, OdinSerialize]
        public bool FaceDirection { get; private set; }

        #region FaceFrom

        [TabGroup("Direction")]
        [ShowIf("@" + nameof(FaceDirection))]
        [ShowInInspector, OdinSerialize]
        public bool FromParticlePosition { get; private set; } = true;

        [TabGroup("Direction")]
        [ShowIf("@" + nameof(FaceDirection)
                    + " && !" + nameof(FromParticlePosition))]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget FacingFrom { get; private set; }

        [TabGroup("Direction")]
        [ShowIf("@" + nameof(FaceDirection)
                    + " && " + nameof(FacingFrom) + " == AnimationTarget.CellGroup"
                    + " && !" + nameof(FromParticlePosition))]
        [ShowInInspector, OdinSerialize]
        public int FacingFromGroup { get; private set; }

        [TabGroup("Direction")]
        [ShowIf("@" + nameof(FaceDirection)
                    + " && " + nameof(FacingFrom) + " == AnimationTarget.CellGroup"
                    + " && !" + nameof(FromParticlePosition))]
        [ShowInInspector, OdinSerialize]
        public CellPriority FacingFromCell { get; private set; }

        #endregion

        #region FaceTo

        [TabGroup("Direction")]
        [ShowIf("@" + nameof(FaceDirection))]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget FacingTo { get; private set; }

        [TabGroup("Direction")]
        [ShowIf("@" + nameof(FaceDirection) + " && " + nameof(FacingTo) + " == AnimationTarget.CellGroup")]
        [ShowInInspector, OdinSerialize]
        public int FacingToGroup { get; private set; }

        [TabGroup("Direction")]
        [ShowIf("@" + nameof(FaceDirection) + " && " + nameof(FacingTo) + " == AnimationTarget.CellGroup")]
        [ShowInInspector, OdinSerialize]
        public CellPriority FacingToCell { get; private set; }

        #endregion

        [TabGroup("Animation")]
        [ShowInInspector, OdinSerialize]
        public bool RemapAnimationTime { get; private set; }

        [TabGroup("Animation")]
        [ShowIf("@" + nameof(RemapAnimationTime))]
        [PropertyOrder(float.MaxValue - 1)]
        [ShowInInspector, OdinSerialize]
        public float RemappedTime { get; private set; }

        [TabGroup("Animation")]
        [PropertyOrder(float.MaxValue)]
        [ShowInInspector]
        public int AnimationLoops
        {
            get => _animationLoops;
            private set
            {
                if (value < 1) value = 1;
                _animationLoops = value;
            }
        }

        protected override async UniTask OnAnimationPlayRequest(AnimationPlayContext context, CancellationToken cancellationToken)
        {
            var mapView = context.SceneContext.BattleMapView;

            if (SpawnAt == AnimationTarget.CellGroup)
            {
                var spawnPositions = context.TargetedCellGroups.GetGroup(SpawnGroup);
                var animations = new List<UniTask>();
                var particles = new List<AnimatedParticle>();
                foreach (var position in spawnPositions)
                {
                    var animTask = SpawnParticle(context, position, mapView, cancellationToken, out var particle);
                    particles.Add(particle);
                    animations.Add(animTask);
                }

                await UniTask.WhenAll(animations);
                foreach (var particle in particles)
                    context.SceneContext.ParticlesPool.Release(particle);
            }
            else if (SpawnAt == AnimationTarget.Caster)
            {
                await SpawnParticle(context, context.Caster.Position, mapView, cancellationToken, out var particle);
                context.SceneContext.ParticlesPool.Release(particle);
            }
            else if (SpawnAt == AnimationTarget.Target)
            {
                if (context.Target.IsDisposedFromBattle)
                    return;
                await SpawnParticle(context, context.Target.Position, mapView, cancellationToken, out var particle);
                context.SceneContext.ParticlesPool.Release(particle);
            }
            else
                Logging.LogException(new NotImplementedException());
        }

        private UniTask SpawnParticle(
            AnimationPlayContext context,
            Vector2Int gamePosition,
            BattleMapView mapView,
            CancellationToken cancellationToken,
            out AnimatedParticle particle)
        {
            particle = context.SceneContext.ParticlesPool.Create(ParticleType);
            var particlePosZ = particle.transform.position.z;
            var cellVisualPos = mapView.GameToWorldPosition(gamePosition);
            particle.transform.position = new Vector3(cellVisualPos.x, cellVisualPos.y, particlePosZ);

            if (FaceDirection)
            {
                Vector2 facingFromGamePos;
                if (FromParticlePosition)
                {
                    facingFromGamePos = gamePosition;
                }
                else
                {
                    facingFromGamePos = FacingFrom.SelectGamePosition(
                        context.Caster.Position,
                        context.TargetGamePosition,
                        context.TargetedCellGroups,
                        FacingFromGroup,
                        FacingFromCell);
                }

                Vector2 facingToGamePos = FacingTo.SelectGamePosition(
                    context.Caster.Position,
                    context.TargetGamePosition,
                    context.TargetedCellGroups,
                    FacingToGroup,
                    FacingToCell);
                var direction = facingToGamePos - facingFromGamePos;
                particle.transform.right = direction;
            }

            switch (Follow)
            {
                case ParticleFollowOption.DontFollow:
                    break;
                case ParticleFollowOption.CasterEntity:
                    particle.StartFollowing(context.CasterView.transform);
                    break;
                case ParticleFollowOption.TargetEntity:
                    if (context.TargetView == null)
                    {

                    }
                    particle.StartFollowing(context.TargetView.transform);
                    break;
                default:
                {
                    Logging.LogException(new NotImplementedException());
                    throw new NotImplementedException();
                }
            }

            UniTask animationTask;
            if (RemapAnimationTime)
                animationTask = particle.PlayTimeRemappedAnimation(RemappedTime, AnimationLoops, cancellationToken);
            else
                animationTask = particle.PlayAnimation(AnimationLoops, cancellationToken);
            return animationTask;
        }

        public enum ParticleFollowOption
        {
            DontFollow,
            CasterEntity,
            TargetEntity
        }
    }
}