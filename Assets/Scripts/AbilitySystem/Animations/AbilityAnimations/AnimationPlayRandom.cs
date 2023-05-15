using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrderElimination.AbilitySystem.Animations
{
    public class AnimationPlayRandom : IAbilityAnimation
    {
        [ShowInInspector, OdinSerialize]
        public List<IAbilityAnimation> Animations { get; set; } = new();

        public async UniTask Play(AnimationPlayContext context)
        {
            if (Animations.Count == 0)
                return;
            var animationId = Random.Range(0, Animations.Count);
            await Animations[animationId].Play(context);
        }
    }
}
