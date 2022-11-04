using UnityEngine;
using UnityEngine.UI;

namespace CharacterAbility
{
    public class AbilityButton : Button
    {
        private AbilityView _abilityView;
        
        public void SetAbility(AbilityView abilityView)
        {
            image.sprite = abilityView.AbilityIcon;
            _abilityView = abilityView;
            interactable = true;
            onClick.AddListener(_abilityView.Clicked);
        }
        
        public void ResetAbility()
        {
            image.sprite = null;
            _abilityView = null;
            interactable = false;
            onClick.RemoveListener(_abilityView.Clicked);
        }
        
        
        protected override void OnDisable()
        {
            base.OnDisable();
            onClick.RemoveAllListeners();
        }
    }
}