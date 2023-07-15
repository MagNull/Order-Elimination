using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Threading;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    [CreateAssetMenu(fileName = "AbilityAnimationPreset", menuName = "OrderElimination/AbilitySystem/Animations/AnimationPreset")]
    public class AnimationPreset : SerializedScriptableObject
    {
        [ShowInInspector, NonSerialized]
        [PreviewField(300, ObjectFieldAlignment.Left), ReadOnly]
        private static Animation _animationPreview;

        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation Animation { get; private set; }

        public async UniTask Play(
            AnimationPlayContext context, 
            CancellationToken cancellationToken)
        {
            await Animation.Play(context).AttachExternalCancellation(cancellationToken);
        }

        public static explicit operator AwaitableAbilityAnimation(AnimationPreset animationPreset)
        {
            return new AnimationPresetWrapper(animationPreset);
        }
    }
}
