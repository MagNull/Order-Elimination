using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public enum DefaultAnimation
    {
        Miss = 1,
        Evasion = 2,
        //Walk,
        //Evasion,
        //CloakIn
        //CloakOut
        //Damaged
    }

    public class DefaultAnimationsPool : SerializedMonoBehaviour
    {
        [ShowInInspector, OdinSerialize]
        private Dictionary<DefaultAnimation, IAbilityAnimation> _defaultAnimations = new();

        public bool IsAnimationAssigned(DefaultAnimation animation) => _defaultAnimations.ContainsKey(animation);

        public IAbilityAnimation this[DefaultAnimation animation] => _defaultAnimations[animation];

        public IAbilityAnimation GetDefaultAnimation(DefaultAnimation animation)
        {
            if (!IsAnimationAssigned(animation))
                Logging.LogException( new NotImplementedException($"Default animation of {animation} was not found."));
            return _defaultAnimations[animation];
        }
    }
}
