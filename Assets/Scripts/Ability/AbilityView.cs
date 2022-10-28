using UnityEngine;
using UnityEngine.UI;

namespace Ability
{
    public class AbilityView
    {
        [SerializeField]
        private readonly Sprite _abilityIcon;
        private readonly IAbility _ability;

        public AbilityView(IAbility ability, AbilityInfo info)
        {
            _ability = ability;
            _abilityIcon = info.Icon;
        }

        public Sprite AbilityIcon => _abilityIcon;

        public void OnUsed()
        {
            
        }

        public void Clicked()
        {
            
        }
    }
}