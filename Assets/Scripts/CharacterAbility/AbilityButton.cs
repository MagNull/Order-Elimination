using UnityEngine;
using UnityEngine.UI;

namespace CharacterAbility
{
    [RequireComponent(typeof(Button))]
    public class AbilityButton : MonoBehaviour
    {
        private AbilityView _abilityView;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void SetAbility(AbilityView abilityView)
        {
            ResetAbility();
            _button.image.sprite = abilityView.AbilityIcon;
            _abilityView = abilityView;
            _button.interactable = true;
            _button.onClick.AddListener(_abilityView.Clicked);
        }
        
        private void ResetAbility()
        {
            _button.image.sprite = null;
            _abilityView = null;
            _button.interactable = false;
            _button.onClick.RemoveAllListeners();
        }
        
        
        protected void OnDisable()
        {
            ResetAbility();
        }
    }
}