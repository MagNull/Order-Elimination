using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EffectView : IReadOnlyEffectView
    {
        [OdinSerialize]
        public string Name { get; set; }

        [OdinSerialize]
        public Sprite Icon { get; set; }

        [OdinSerialize]
        public bool IsHidden { get; set; }

        [OdinSerialize]
        public IAbilityAnimation AnimationOnActivation { get; set; }

        [OdinSerialize]
        public IAbilityAnimation AnimationOnDeactivation { get; set; }
    }

    public interface IReadOnlyEffectView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public bool IsHidden { get; }
        public IAbilityAnimation AnimationOnActivation { get; }
        public IAbilityAnimation AnimationOnDeactivation { get; }
    }
}
