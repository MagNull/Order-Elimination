using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationDelay : IAbilityAnimation
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

        public async UniTask Play(AnimationPlayContext context)
        {
            var timeInMilliseconds = Mathf.RoundToInt(DelayTime * 1000);
            await UniTask.Delay(timeInMilliseconds);
        }
    }
}
