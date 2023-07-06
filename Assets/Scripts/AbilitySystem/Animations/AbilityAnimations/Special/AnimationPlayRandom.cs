using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Threading;
using Random = UnityEngine.Random;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationPlayRandom : AwaitableAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public List<IAbilityAnimation> Animations { get; set; } = new();

        protected override async UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken)
        {
            if (Animations.Count == 0)
                return;
            var animationId = Random.Range(0, Animations.Count);
            await Animations[animationId].Play(context).AttachExternalCancellation(cancellationToken);
        }
    }
}
