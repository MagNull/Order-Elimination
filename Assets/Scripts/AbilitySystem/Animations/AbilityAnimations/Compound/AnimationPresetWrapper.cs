﻿using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading;

namespace OrderElimination.AbilitySystem
{
    public class AnimationPresetWrapper : AwaitableAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public AnimationPreset AnimationPreset { get; private set; }

        protected override async UniTask OnAnimationPlayRequest(AnimationPlayContext context, CancellationToken cancellationToken)
        {
            await AnimationPreset.Play(context, cancellationToken);
        }

        public AnimationPresetWrapper(AnimationPreset animationPreset)
        {
            AnimationPreset = animationPreset;
        }
    }
}
