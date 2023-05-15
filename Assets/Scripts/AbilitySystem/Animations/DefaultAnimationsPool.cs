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
    public enum DefaultAnimation
    {
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

        public IAbilityAnimation GetDefaultAnimation(DefaultAnimation animation)
        {
            if (_defaultAnimations.ContainsKey(animation))
                throw new NotImplementedException($"Default animation of {animation} was not found.");
            return _defaultAnimations[animation];
        }
    }
}
