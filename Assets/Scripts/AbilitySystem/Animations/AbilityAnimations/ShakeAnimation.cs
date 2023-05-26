using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem.Animations
{
    public class ShakeAnimation : IAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public AnimationTarget ShakeTarget { get; set; }

        [ShowInInspector, OdinSerialize]
        public float ShakeX { get; set; } = 0.5f;

        [ShowInInspector, OdinSerialize]
        public float ShakeY { get; set; } = 0.5f;

        [ShowInInspector, OdinSerialize]
        public int Vibrations { get; set; } = 10;

        [ShowInInspector, OdinSerialize]
        public float Duration { get; set; } = 1;

        public async UniTask Play(AnimationPlayContext context)
        {
            if (ShakeTarget == AnimationTarget.Target)
                await context.TargetView.Shake(ShakeX, ShakeY, Duration, Vibrations);
            else if (ShakeTarget == AnimationTarget.Caster)
                await context.CasterView.Shake(ShakeX, ShakeY, Duration, Vibrations);
        }
    }
}
