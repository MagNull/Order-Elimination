﻿using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.AbilitySystem.Animations.AbilityAnimations.Special
{
    public class AnimationSound : AwaitableAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public AudioClip[] Sounds { get; set; } = new AudioClip[0];

        [ShowInInspector, OdinSerialize]
        public bool WaitForCompletion { get; set; }

        protected override async UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken)
        {
            if (Sounds.Length == 0)
            {
                Logging.LogError(new InvalidOperationException("No audioclips to play."));
                return;
            }
            if (WaitForCompletion)
                await PlaySound(context.SceneContext.SoundEffectsPlayer, cancellationToken);
            else
                PlaySound(context.SceneContext.SoundEffectsPlayer, cancellationToken);
        }

        private async UniTask PlaySound(SoundEffectsPlayer player, CancellationToken cancellationToken)
        {
            var clip = Sounds[Random.Range(0, Sounds.Length)];//max is exclusive!
            var playingDuration = clip.length;
            var duration = Mathf.RoundToInt(playingDuration * 1000);
            player.PlaySound(clip);
            await UniTask.Delay(duration, cancellationToken: cancellationToken);
        }
    }
}
