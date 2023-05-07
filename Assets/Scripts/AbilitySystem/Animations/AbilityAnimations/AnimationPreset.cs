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
    [CreateAssetMenu(fileName = "AbilityAnimationPreset", menuName = "AbilitySystem/Animations/AnimationPreset")]
    public class AnimationPreset : SerializedScriptableObject, IAbilityAnimation
    {
        [ShowInInspector, NonSerialized]
        [PreviewField(300, ObjectFieldAlignment.Left), ReadOnly]
        private static Animation _animationPreview;

        [ShowInInspector, OdinSerialize]
        public IAbilityAnimation Animation { get; private set; }

        public async UniTask Play(AnimationPlayContext context)
        {
            await Animation.Play(context);
        }
    }
}
