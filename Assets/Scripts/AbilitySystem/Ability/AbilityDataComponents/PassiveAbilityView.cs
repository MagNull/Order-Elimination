using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        //public VideoClip PreviewVideo { get; }
        public bool HideInCharacterDiscription { get; }

        public PassiveAbilityView(
            string name, Sprite icon, string description,
            bool hideInCharacterDiscription)
        {
            Name = name;
            Icon = icon;
            Description = description ?? "";
            HideInCharacterDiscription = hideInCharacterDiscription;
        }
    }
}
