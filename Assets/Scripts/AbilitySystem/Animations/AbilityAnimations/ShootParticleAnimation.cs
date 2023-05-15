using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.AbilitySystem.Animations
{

    public class ShootParticleAnimation : IAbilityAnimation
    {
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

        [ShowInInspector, OdinSerialize]
        public ParticleType BulletParticle { get; set; }

        [ShowInInspector, OdinSerialize, LabelText("Move By Constant")]
        public MoveByConstant MoveBy { get; set; }

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

        [ShowInInspector, OdinSerialize]
        public Ease MovementEase { get; set; } = Ease.Flash;

        [ShowInInspector, OdinSerialize]
        public bool FaceDirection { get; set; }

        [ShowInInspector, OdinSerialize]
        public bool Inverse { get; set; }

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

        [ShowInInspector, OdinSerialize]
        public bool RemapAnimationTime { get; set; }

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
        //bool ThenReturn
        //bool AwaitReturn

        public async UniTask Play(AnimationPlayContext context)
        {
            if (!context.CasterGamePosition.HasValue
                || !context.TargetGamePosition.HasValue)
                throw new ArgumentException();

            var bullet = context.SceneContext.ParticlesPool.Create(BulletParticle);
            var mapView = context.SceneContext.BattleMapView;
            var angle = Random.Range(0, 2 * Mathf.PI);
            var distanceFromCenter = Random.Range(MinSpread, MaxSpread);
            var randomOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distanceFromCenter;

            var start = context.CasterGamePosition.Value;
            var destination = context.TargetGamePosition.Value + randomOffset;
            var realWorldStart = mapView.GameToWorldPosition(start);
            var realWorldDestination = mapView.GameToWorldPosition(destination);
            var realWorldOffset = realWorldDestination - realWorldStart;

            var from = new Vector3(realWorldStart.x, realWorldStart.y, bullet.transform.position.z);
            var to = new Vector3(realWorldDestination.x, realWorldDestination.y, bullet.transform.position.z);
            var direction = realWorldOffset;
            var time = MoveBy switch
            {
                MoveByConstant.Time => Time,
                MoveByConstant.Speed => realWorldOffset.magnitude / Speed,
                _ => throw new NotImplementedException(),
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
                bullet.PlayTimeRemappedAnimation(time, AnimationLoops);
            else
                bullet.PlayAnimation(AnimationLoops);
            await bullet.transform
                .DOMove(to, time)
                .SetEase(MovementEase)
                .AsyncWaitForCompletion();
            context.SceneContext.ParticlesPool.Release(bullet);
        }
    }
}
