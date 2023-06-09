using System;
using CharacterAbility;

namespace RoguelikeMap.UI.Abilities
{
    public class PassiveAbilityButton : AbilityButton
    {
        public event Action OnClick;
        
        public void SetAbilityInfos(AbilityInfo info)
        {
            _abilityImage.sprite = info.Icon;
        }

        public override void OnMouseDown()
        {
            OnClick?.Invoke();
        }
    }
}