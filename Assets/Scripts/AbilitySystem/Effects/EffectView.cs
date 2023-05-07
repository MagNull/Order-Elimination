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
    public class EffectView
    {
        [ShowInInspector, OdinSerialize]
        public string Name { get; private set; }

        [ShowInInspector, OdinSerialize]
        [PreviewField]
        public Sprite Icon { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool IsHidden { get; private set; }
    }
}
