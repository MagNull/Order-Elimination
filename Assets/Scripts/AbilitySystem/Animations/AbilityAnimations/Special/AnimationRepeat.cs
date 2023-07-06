using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationRepeat : AwaitableAbilityAnimation
    {
        [HideInInspector, OdinSerialize]
        private int _repeatAmount;

        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation Animation { get; set; }

        [ShowInInspector]
        public int RepeatAmount
        {
            get => _repeatAmount;
            set
            {
                if (value < 0) value = 0;
                _repeatAmount = value;
            }
        }

        [ShowInInspector, OdinSerialize]
        public bool PlaySequentially { get; set; } = true;

        [ShowInInspector, OdinSerialize]
        public bool WaitForCompletion { get; set; } = true;

        protected override async UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken)
        {
            if (WaitForCompletion)
            {

                if (PlaySequentially)
                    await RepeatAnimation(Animation, context, RepeatAmount, true, cancellationToken);
                else
                    await RepeatAnimation(Animation, context, RepeatAmount, false, cancellationToken);
            }
            else
            {
                if (PlaySequentially)
                    RepeatAnimation(Animation, context, RepeatAmount, true, cancellationToken);
                else
                    RepeatAnimation(Animation, context, RepeatAmount, false, cancellationToken);

            }
        }

        private async UniTask RepeatAnimation(
            IAbilityAnimation animation, 
            AnimationPlayContext context, 
            int repeatCount, 
            bool waitEach,
            CancellationToken cancellationToken)
        {
            var playingAnimations = new List<UniTask>();
            for (var i = 0; i < repeatCount; i++)
            {
                if (waitEach)
                    await animation.Play(context)
                        .AttachExternalCancellation(cancellationToken);
                else
                {
                    playingAnimations.Add(Animation.Play(context)
                        .AttachExternalCancellation(cancellationToken));
                }
            }
            await UniTask.WhenAll(playingAnimations);
        }
    }
}
