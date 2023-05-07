using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using Unity.Services.Analytics;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimatedParticle : SerializedMonoBehaviour
    {
        public event Action PlaybackRequested;
        public event Action PlaybackStopped;
        public event Action<float> TimedPlaybackRequested;

        public async UniTask PlayAnimation()
        {
            StopAnimation();
            PlaybackRequested?.Invoke();
            await OnPlayRequest();
        }

        /// <summary>
        /// Plays a particle animation scaled to a given timeSpan.
        /// </summary>
        /// <param name="timeSpan"></param>
        public async UniTask PlayAnimationScaledToTime(float timeSpan)
        {
            TimedPlaybackRequested?.Invoke(timeSpan);
            await OnTimedPlayRequest(timeSpan);
        }

        public void StopAnimation()
        {
            Stop();
            PlaybackStopped?.Invoke();
        }

        protected virtual async UniTask OnPlayRequest() { }

        protected virtual async UniTask OnTimedPlayRequest(float timeSpan) { }

        protected virtual void Stop() { }
    }
}
