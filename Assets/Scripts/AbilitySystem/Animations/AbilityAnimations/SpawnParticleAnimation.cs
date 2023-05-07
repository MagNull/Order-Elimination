using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem.Animations
{
    public class SpawnParticleAnimation : IAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public ParticleType ParticleType;

        [ShowInInspector, OdinSerialize]
        public bool ScaleToFixedTime;

        [ShowInInspector, OdinSerialize, ShowIf("@" + nameof(ScaleToFixedTime))]
        public float TimeSpan;

        public async UniTask Play(AnimationPlayContext context)
        {
            var particle = context.SceneContext.ParticlesPool.Create(ParticleType);
            if (ScaleToFixedTime)
                await particle.PlayAnimationScaledToTime(TimeSpan);
            else
                await particle.PlayAnimation();
            context.SceneContext.ParticlesPool.Release(particle);
        }
    }
}
