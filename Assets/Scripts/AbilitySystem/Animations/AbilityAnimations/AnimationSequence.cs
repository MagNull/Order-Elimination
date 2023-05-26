using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationSequence : IAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public bool PlaySequentially { get; set; }

        [ShowInInspector, OdinSerialize]
        public bool WaitForCompletion { get; set; }

        [ShowInInspector, OdinSerialize]
        public List<IAbilityAnimation> Animations { get; set; } = new();

        public async UniTask Play(AnimationPlayContext context)
        {
            if (WaitForCompletion)
            {
                if (PlaySequentially)
                {
                    await PlayAnimationsSequentially(Animations, context);
                }
                else
                {
                    await UniTask.WhenAll(Animations.Select(a => a.Play(context)));
                }
            }
            else
            {
                if (PlaySequentially)
                {
                    PlayAnimationsSequentially(Animations, context);
                }
                else
                {
                    UniTask.WhenAll(Animations.Select(a => a.Play(context)));
                }
            }
        }

        private async UniTask PlayAnimationsSequentially(IEnumerable<IAbilityAnimation> animations, AnimationPlayContext context)
        {
            foreach (var animation in animations)
            {
                await animation.Play(context);
            }
        }
    }
}
