using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Animations;
using static Unity.VisualScripting.Member;

namespace OrderElimination.AbilitySystem.Animations
{
    [RequireComponent(typeof(PositionConstraint))]
    public class AnimatedParticle : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private Animator _animator;
        [OdinSerialize]
        private PositionConstraint _positionConstraint;
        [Tooltip("Used to hide the body of a particle.")]
        [OdinSerialize]
        private GameObject _particleBody;
        [OdinSerialize]
        private List<TrailRenderer> _trailRenderers = new();

        public bool HasTrails
        {
            get
            {
                if (_trailRenderers == null) return false;
                return _trailRenderers.Any(t => t.positionCount > 0);
            }
        }

        public event Action PlaybackRequested;
        public event Action PlaybackStopped;
        public event Action<float> TimedPlaybackRequested;

        public async UniTask PlayAnimation(float loops = 1)
        {
            StopAnimation();
            PlaybackRequested?.Invoke();
            await OnPlayRequest(loops);
        }

        /// <summary>
        /// Plays a particle animation remapped to a given time in seconds.
        /// </summary>
        /// <param name="remappedTime"></param>
        public async UniTask PlayTimeRemappedAnimation(float remappedTime, float loops = 1)
        {
            StopAnimation();
            TimedPlaybackRequested?.Invoke(remappedTime);
            await OnTimedPlayRequest(remappedTime, loops);
        }

        public virtual void StopAnimation()
        {
            if (_animator != null)
            {
                _animator.StopPlayback();
            }
            PlaybackStopped?.Invoke();
        }

        public virtual void SetBodyVisibility(bool isVisible)
        {
            if (_particleBody != null)
                _particleBody.SetActive(isVisible);
        }

        public void StartFollowing(Transform transform)
        {
            //Logging.Log("Start follow" , Colorize.Yellow);
            var source = new ConstraintSource();
            source.sourceTransform = transform;
            source.weight = 1;
            var sources = new List<ConstraintSource>() { source };
            _positionConstraint.SetSources(sources);
        }

        public void StopFollowing()
        {
            //Logging.Log("Stop follow" , Colorize.Yellow);
            if (_positionConstraint != null)
                _positionConstraint.SetSources(new List<ConstraintSource>());
        }

        protected virtual async UniTask OnPlayRequest(float loops)
        {
            if (_animator != null)
            {
                _animator.Play(0);
                var stateLength = _animator.GetCurrentAnimatorStateInfo(0).length;
                _animator.speed = 1;
                await UniTask.Delay(Mathf.RoundToInt(stateLength * loops * 1000));
            }
        }

        protected virtual async UniTask OnTimedPlayRequest(float timeSpan, float loops)
        {

            if (_animator != null)
            {
                _animator.Play(0);
                var stateLength = _animator.GetCurrentAnimatorStateInfo(0).length;
                _animator.speed = stateLength / timeSpan * loops;
                await UniTask.Delay(Mathf.RoundToInt(timeSpan * 1000));
            }
        }
    }
}
