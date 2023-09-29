using Cysharp.Threading.Tasks;
using DG.Tweening;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.AbilitySystem.Animations
{

    public class ShootParticleAnimation : AwaitableAbilityAnimation
    {
        private bool OriginAsCellGroup => OriginTarget == AnimationTarget.CellGroup;
        private bool DestinationAsCellGroup => DestinationTarget == AnimationTarget.CellGroup;

        [HideInInspector, OdinSerialize]
        private float _minSpread;
        [HideInInspector, OdinSerialize]
        private float _maxSpread;
        [HideInInspector, OdinSerialize]
        private float _time;
        [HideInInspector, OdinSerialize]
        private float _speed;
        [HideInInspector, OdinSerialize]
        private int _animationLoops = 1;

        [BoxGroup("Particle Settings", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        public ParticleType BulletParticle { get; set; }

        [BoxGroup("Particle Settings")]
        [ShowInInspector, OdinSerialize]
        public bool RemapAnimationTime { get; set; }

        [BoxGroup("Particle Settings")]
        [ShowInInspector]
        public int AnimationLoops
        {
            get => _animationLoops;
            set
            {
                if (value < 1) value = 1;
                _animationLoops = value;
            }
        }

        #region End Points
        [BoxGroup("Origin", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget OriginTarget { get; set; } = AnimationTarget.Caster;

        [BoxGroup("Origin")]
        [EnableIf("@" + nameof(OriginAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public int OriginCellGroup { get; set; }

        [BoxGroup("Origin")]
        [EnableIf("@" + nameof(OriginAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public CellPriority OriginCellPriority { get; set; }

        [BoxGroup("Destination", CenterLabel = true)]
        [ShowInInspector, OdinSerialize]
        public AnimationTarget DestinationTarget { get; set; } = AnimationTarget.Target;

        [BoxGroup("Destination")]
        [EnableIf("@" + nameof(DestinationAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public int DestinationCellGroup { get; set; }

        [BoxGroup("Destination")]
        [EnableIf("@" + nameof(DestinationAsCellGroup))]
        [ShowInInspector, OdinSerialize]
        public CellPriority DestinationCellPriority { get; set; }

        [BoxGroup("Destination")]
        [ShowInInspector, MinValue(0), MaxValue(0.5f)]
        public float MinSpread
        {
            get => _minSpread;
            set
            {
                if (value < 0) value = 0;
                if (value > MaxSpread) value = MaxSpread;
                _minSpread = value;
            }
        }

        [BoxGroup("Destination")]
        [ShowInInspector, MinValue(0), MaxValue(0.5f)]
        public float MaxSpread
        {
            get => _maxSpread;
            set
            {
                if (value < MinSpread) value = MinSpread;
                if (value > 0.5f) value = 0.5f;
                _maxSpread = value;
            }
        }
        #endregion

        [BoxGroup("Movement", CenterLabel = true)]
        [ShowInInspector, OdinSerialize, LabelText("Move By Constant")]
        public MoveByConstant MoveBy { get; set; }

        [BoxGroup("Movement")]
        [ShowInInspector, ShowIf("@MoveBy == MoveByConstant.Speed")]
        public float Speed
        {
            get => _speed;
            set
            {
                if (value < 0) value = 0;
                _speed = value;
            }
        }

        [BoxGroup("Movement")]
        [ShowInInspector, ShowIf("@MoveBy == MoveByConstant.Time")]
        public float Time
        {
            get => _time;
            set
            {
                if (value < 0) value = 0;
                _time = value;
            }
        }

        [BoxGroup("Movement")]
        [ShowInInspector, OdinSerialize]
        public Ease MovementEase { get; set; } = Ease.Flash;

        [BoxGroup("Movement")]
        [ShowInInspector, OdinSerialize]
        public bool FaceDirection { get; set; }

        [BoxGroup("Movement")]
        [ShowInInspector, OdinSerialize]
        public bool Inverse { get; set; }

        //bool ThenReturn
        //bool AwaitReturn

        protected override async UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken)
        {
            if (!context.CasterGamePosition.HasValue
                || !context.TargetGamePosition.HasValue)
                Logging.LogException(new ArgumentException());

            var bullet = context.SceneContext.ParticlesPool.Create(BulletParticle);
            var mapView = context.SceneContext.BattleMapView;

            //Origin
            var origin = OriginTarget switch
            {
                AnimationTarget.Target => context.TargetGamePosition.Value,
                AnimationTarget.Caster => context.CasterGamePosition.Value,
                AnimationTarget.CellGroup => GetPositionByPriority(
                    context.TargetedCellGroups, OriginCellGroup, OriginCellPriority, 
                    context.CasterGamePosition, context.TargetGamePosition),
                _ => throw new NotImplementedException(),
            };
            var realWorldStart = mapView.GameToWorldPosition(origin);

            //Destination
            var angle = Random.Range(0, 2 * Mathf.PI);
            var distanceFromCenter = Random.Range(MinSpread, MaxSpread);
            var randomOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distanceFromCenter;
            var destination = DestinationTarget switch
            {
                AnimationTarget.Target => context.TargetGamePosition.Value,
                AnimationTarget.Caster => context.CasterGamePosition.Value,
                AnimationTarget.CellGroup => GetPositionByPriority(
                    context.TargetedCellGroups, OriginCellGroup, DestinationCellPriority,
                    context.CasterGamePosition, context.TargetGamePosition),
                _ => throw new NotImplementedException(),
            } + randomOffset;
            var realWorldDestination = mapView.GameToWorldPosition(destination);

            var realWorldOffset = realWorldDestination - realWorldStart;

            var from = new Vector3(realWorldStart.x, realWorldStart.y, bullet.transform.position.z);
            var to = new Vector3(realWorldDestination.x, realWorldDestination.y, bullet.transform.position.z);
            var direction = realWorldOffset;
            var time = MoveBy switch
            {
                MoveByConstant.Time => Time,
                MoveByConstant.Speed => realWorldOffset.magnitude / Speed,
                _ => throw new NotImplementedException()
            };
            if (Inverse)
            {
                var t = to;
                to = from;
                from = t;
            }
            bullet.transform.position = from;
            if (FaceDirection)
                bullet.transform.right = direction;
            if (RemapAnimationTime)
                bullet.PlayTimeRemappedAnimation(time, AnimationLoops, cancellationToken);
            else
                bullet.PlayAnimation(AnimationLoops, cancellationToken);
            await bullet.transform
                .DOMove(to, time)
                .SetEase(MovementEase)
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
            context.SceneContext.ParticlesPool.Release(bullet);

            Vector2Int GetPositionByPriority(
                CellGroupsContainer cellGroups, int group, CellPriority priority,
                Vector2Int? casterPosition, Vector2Int? targetPosition)
            {
                var positions = cellGroups.GetGroup(group);
                if (positions.Length == 0)
                    throw new InvalidOperationException("No origin position");
                return priority.GetPositionByPriority(positions, casterPosition, targetPosition);
            }
        }
    }
}
