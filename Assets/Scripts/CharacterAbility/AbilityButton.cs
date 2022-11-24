using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterAbility
{
    [RequireComponent(typeof(Button))]
    public class AbilityButton : MonoBehaviour
    {
        public event Action Casted;
        public event Action<AbilityButton> Clicked;
        private AbilityView _abilityView;
        [SerializeField]
        private Image _abilityImage;
        [SerializeField]
        private TextMeshProUGUI _abilityName;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.interactable = false;
        }

        public Type GetAbilityType() => _abilityView.AbilityType;

        public void SetAbility(AbilityView abilityView)
        {
            RemoveAbility();
            _abilityName.text = abilityView.Name;
            _abilityImage.sprite = abilityView.AbilityIcon;
            _abilityView = abilityView;
            _abilityView.Casted += OnCasted;
            CheckUsePossibility();
            _button.onClick.AddListener(OnClicked);
        }

        public void CancelAbilityCast() => _abilityView?.CancelCast();

        public void OnClicked()
        {
            Clicked?.Invoke(this);
            _abilityView.Clicked();
        }
        
        private void OnCasted() => Casted?.Invoke();

        public void CheckUsePossibility()
        {
            if(_abilityView.CanCast)
            {
                _button.interactable = true;
                return;
            }
            _button.interactable = false;
        }

        public void RemoveAbility()
        {
            CancelAbilityCast();
            _abilityImage.sprite = null;
            _abilityName.text = "";
            if(_abilityView != null)
                _abilityView.Casted -= OnCasted;
            _abilityView = null;
            _button.interactable = false;
            _button.onClick.RemoveAllListeners();
        }
    }
}