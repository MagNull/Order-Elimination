using UnityEngine;
using UnityEngine.UI;

namespace Ability
{
    public class AbilityButton : Button
    {
        [SerializeField]
        private Image _icon;
        private AbilityView _abilityView;

        public void SetAbility(AbilityView abilityView)
        {
            _icon.sprite = abilityView.AbilityIcon;
            _abilityView = abilityView;
            interactable = true;
            onClick.AddListener(_abilityView.Clicked);
        }
        
        public void ResetAbility()
        {
            _icon.sprite = null;
            _abilityView = null;
            interactable = false;
            onClick.RemoveListener(_abilityView.Clicked);
        }
        
        
        protected override void OnDisable()
        {
            base.OnDisable();
            onClick.RemoveListener(_abilityView.Clicked);
        }
    }
}