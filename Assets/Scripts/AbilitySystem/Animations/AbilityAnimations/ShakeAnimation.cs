using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Threading;

namespace OrderElimination.AbilitySystem.Animations
{
    public class ShakeAnimation : AwaitableAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public ActionEntity ShakingEntity { get; set; }

        [ShowInInspector, OdinSerialize]
        public float ShakeX { get; set; } = 0.5f;

        [ShowInInspector, OdinSerialize]
        public float ShakeY { get; set; } = 0.5f;

        [ShowInInspector, OdinSerialize]
        public int Vibrations { get; set; } = 10;

        [ShowInInspector, OdinSerialize]
        public float Duration { get; set; } = 1;

        protected override async UniTask OnAnimationPlayRequest(AnimationPlayContext context, CancellationToken cancellationToken)
        {
            if (ShakingEntity == ActionEntity.Target)
                await context.TargetView.Shake(ShakeX, ShakeY, Duration, Vibrations)
                    .AttachExternalCancellation(cancellationToken);
            else if (ShakingEntity == ActionEntity.Caster)
                await context.CasterView.Shake(ShakeX, ShakeY, Duration, Vibrations)
                    .AttachExternalCancellation(cancellationToken);
        }
    }
}
