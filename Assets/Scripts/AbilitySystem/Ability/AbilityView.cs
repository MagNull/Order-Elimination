using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public class AbilityView
    {
        [SerializeField, ShowInInspector]
        public string Name { get; set; }
        [SerializeField, ShowInInspector]
        public Sprite Icon { get; set; }
        [SerializeField, ShowInInspector]
        public string Description { get; set; }
    }
}
