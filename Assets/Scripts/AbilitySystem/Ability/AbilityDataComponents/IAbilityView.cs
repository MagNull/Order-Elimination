using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IAbilityView
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        public bool HideInCharacterDescription { get; }
    }
}
