using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading;
using UnityEngine;
using static Sentry.MeasurementUnit;

namespace Assets.Scripts.AbilitySystem.Animations.AbilityAnimations.Special
{
    public class AnimationSound : AwaitableAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public AudioClip Sound { get; set; }

        [ShowInInspector, OdinSerialize]
        public float Duration { get; set; } = -1;

        [ShowInInspector, OdinSerialize]
        public bool WaitForCompletion { get; set; }

        protected override async UniTask OnAnimationPlayRequest(
            AnimationPlayContext context, CancellationToken cancellationToken)
        {
            var playingDuration = Duration > 0 ? Duration : Sound.length;
            var duration = Mathf.RoundToInt(playingDuration * 1000);
            if (WaitForCompletion)
                await PlaySound(context.SceneContext.SoundEffectsPlayer, duration, cancellationToken);
            else
                PlaySound(context.SceneContext.SoundEffectsPlayer, duration, cancellationToken);
        }

        private async UniTask PlaySound(SoundEffectsPlayer player, int durationMs, CancellationToken cancellationToken)
        {
            var sound = player.PlaySound(Sound);
            await UniTask.Delay(durationMs, cancellationToken: cancellationToken);
            player.StopSound(sound);
        }
    }
}
