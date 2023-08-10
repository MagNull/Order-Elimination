using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityView : IAbilityView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        //public VideoClip PreviewVideo { get; }
        public bool HideInCharacterDescription { get; }

        public PassiveAbilityView(
            string name, Sprite icon, string description,
            bool hideInCharacterDescription)
        {
            Name = name ?? "";
            Icon = icon;
            Description = description ?? "";
            HideInCharacterDescription = hideInCharacterDescription;
        }
    }
}
