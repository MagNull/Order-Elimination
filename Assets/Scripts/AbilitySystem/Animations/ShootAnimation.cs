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
    public class ShootAnimation : IAbilityAnimation
    {
        private float _bulletSpread;

        [ShowInInspector, OdinSerialize]
        public Particles BulletParticle;

        [ShowInInspector, OdinSerialize]
        public float BulletSpeed { get; private set; }

        [ShowInInspector, OdinSerialize]
        public Ease BulletEase { get; private set; }

        [ShowInInspector, OdinSerialize, MinValue(0), MaxValue(0.5f)]
        public float BulletSpread
        {
            get => _bulletSpread;
            set
            {
                if (value < 0) value = 0;
                if (value > 0.5f) value = 0.5f;
                _bulletSpread = value;
            }
        }

        public bool IsFinished { get; private set; }

        public event Action<IAbilityAnimation> Finished;

        public async UniTask Play(AnimationPlayContext context)
        {
            IsFinished = false;
            if (!context.CasterGamePosition.HasValue
                || !context.TargetGamePosition.HasValue)
                throw new ArgumentException();

            var bullet = context.SceneContext.ParticlesPool.Create(BulletParticle);
            var mapView = context.SceneContext.BattleMapView;
            var randomOffset = new Vector2(
                Random.Range(-BulletSpread, BulletSpread),
                Random.Range(-BulletSpread, BulletSpread));

            var start = context.CasterGamePosition.Value;
            var destination = context.TargetGamePosition.Value + randomOffset;
            var realWorldStart = mapView.GameToWorldPosition(start);
            var realWorldDestination = mapView.GameToWorldPosition(destination);
            var realWorldOffset = realWorldDestination - realWorldStart;

            var from = new Vector3(realWorldStart.x, realWorldStart.y, bullet.transform.position.z);
            var to = new Vector3(realWorldDestination.x, realWorldDestination.y, bullet.transform.position.z);
            var time = realWorldOffset.magnitude / BulletSpeed;

            bullet.transform.position = from;
            bullet.transform.right = realWorldOffset;
            await bullet.transform
                .DOMove(to, time)
                .SetEase(BulletEase)
                .AsyncWaitForCompletion();
            context.TargetView.Shake(0.2f);
            context.SceneContext.ParticlesPool.Release(bullet);
            IsFinished = true;
            Finished?.Invoke(this);
        }
    }
}
