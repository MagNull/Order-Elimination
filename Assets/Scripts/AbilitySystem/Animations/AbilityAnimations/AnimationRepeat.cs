using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationRepeat : IAbilityAnimation
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

        public async UniTask Play(AnimationPlayContext context)
        {
            if (WaitForCompletion)
            {

                if (PlaySequentially)
                    await RepeatAnimation(Animation, context, RepeatAmount, true);
                else
                    await RepeatAnimation(Animation, context, RepeatAmount, false);
            }
            else
            {
                if (PlaySequentially)
                    RepeatAnimation(Animation, context, RepeatAmount, true);
                else
                    RepeatAnimation(Animation, context, RepeatAmount, false);

            }
        }

        private async UniTask RepeatAnimation(IAbilityAnimation animation, AnimationPlayContext context, int repeatCount, bool waitEach)
        {
            var playingAnimations = new List<UniTask>();
            for (var i = 0; i < repeatCount; i++)
            {
                if (waitEach)
                    await animation.Play(context);
                else
                {
                    playingAnimations.Add(Animation.Play(context));
                }
            }
            await UniTask.WhenAll(playingAnimations);
        }
    }
}
