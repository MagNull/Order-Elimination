using System;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterAbility
{
    [RequireComponent(typeof(Button))]
    public class AbilityButton : MonoBehaviour
    {
        public event Action<AbilityButton> Clicked;
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
            _button.onClick.AddListener(OnClicked);
        }

        public void CancelAbilityCast() => _abilityView?.CancelCast();

        public void OnClicked()
        {
            Debug.Log("Clicked");
            Clicked?.Invoke(this);
            _abilityView.Clicked();
        }

        public void ResetAbility()
        {
            CancelAbilityCast();
            _button.image.sprite = null;
            _abilityView = null;
            _button.interactable = false;
            _button.onClick.RemoveAllListeners();
        }

        private void OnDisable()
        {
            ResetAbility();
        }
    }
}