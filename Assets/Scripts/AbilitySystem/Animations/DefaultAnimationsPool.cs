using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public enum DefaultAnimation
    {
        Miss = 1,
        Evasion = 2,
        //DamageReceived = 3,
        //HealthRecovery = 4,
        //ArmorRecovery = 5,
        //Walk,
        //CloakIn
        //CloakOut
    }

    [CreateAssetMenu(fileName = "new Animations Pool", menuName = "OrderElimination/AbilitySystem/Animations/Animations Pool")]
    public class DefaultAnimationsPool : SerializedScriptableObject
    {
        [ShowInInspector, OdinSerialize]
        private Dictionary<DefaultAnimation, IAbilityAnimation> _defaultAnimations = new();

        public bool IsAnimationAssigned(DefaultAnimation animation) => _defaultAnimations.ContainsKey(animation);

        public IAbilityAnimation this[DefaultAnimation animation] => _defaultAnimations[animation];

        public IAbilityAnimation GetDefaultAnimation(DefaultAnimation animation)
        {
            if (!IsAnimationAssigned(animation))
                Logging.LogException(
                    new NotImplementedException($"Default animation of {animation} was not found."));
            return _defaultAnimations[animation];
        }
    }
}
