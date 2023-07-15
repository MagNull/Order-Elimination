using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationSequence : AwaitableAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public bool PlaySequentially { get; set; }

        [ShowInInspector, OdinSerialize]
        public bool WaitForCompletion { get; set; }

        [ShowInInspector, OdinSerialize]
        public List<IAbilityAnimation> Animations { get; set; } = new();

        protected override async UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken)
        {
            if (WaitForCompletion)
            {
                if (PlaySequentially)
                {
                    await PlayAnimationsSequentially(Animations, context, cancellationToken);
                }
                else
                {
                    await UniTask.WhenAll(Animations.Select(a => a.Play(context)
                    .AttachExternalCancellation(cancellationToken)));
                }
            }
            else
            {
                if (PlaySequentially)
                {
                    PlayAnimationsSequentially(Animations, context, cancellationToken);
                }
                else
                {
                    UniTask.WhenAll(Animations.Select(a => a.Play(context)
                    .AttachExternalCancellation(cancellationToken)));
                }
            }
        }

        private async UniTask PlayAnimationsSequentially(
            IEnumerable<IAbilityAnimation> animations, 
            AnimationPlayContext context, 
            CancellationToken cancellationToken)
        {
            foreach (var animation in animations)
            {
                await animation.Play(context).AttachExternalCancellation(cancellationToken);
            }
        }
    }
}
