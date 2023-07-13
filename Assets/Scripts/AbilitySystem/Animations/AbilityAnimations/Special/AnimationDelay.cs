using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Threading;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationDelay : AwaitableAbilityAnimation
    {
        [HideInInspector, OdinSerialize]
        private float _delayTime;

        [ShowInInspector]
        public float DelayTime
        {
            get => _delayTime;
            set
            {
                if (value < 0) value = 0;
                _delayTime = value;
            }
        }

        protected override async UniTask OnAnimationPlayRequest(AnimationPlayContext context, CancellationToken cancellationToken)
        {
            var timeInMilliseconds = Mathf.RoundToInt(DelayTime * 1000);
            await UniTask.Delay(timeInMilliseconds, cancellationToken: cancellationToken);
        }
    }
}
