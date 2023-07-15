using OrderElimination.AbilitySystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [RequireComponent(typeof(HoldableButton))]
    public class SmallAbilityButton : MonoBehaviour
    {
        private IActiveAbilityData _activeAbilityData;
        private IPassiveAbilityData _passiveAbilityData;
        private PassiveAbilityRunner _passiveAbilityRunner;

        [SerializeField] Image _iconComponent;
        [SerializeField] HoldableButton _button;

        [SerializeField]
        private Sprite _noIconAvailableSprite;
        public Sprite NoIconAvailableSprite
        {
            get => _noIconAvailableSprite;
            set
            {
                _noIconAvailableSprite = value;
                if (_iconComponent.sprite != value)
                    _iconComponent.sprite = value;
            }
        }

        public event Action<SmallAbilityButton> Clicked;

        private void Awake()
        {
            _button.ClickAvailable = true;
            _button.HoldAvailable = false;
            _button.interactable = false;
            _button.Clicked += OnClick;
        }

        private void OnClick(HoldableButton button) => Clicked?.Invoke(this);

        public void AssignActiveAbilityData(IActiveAbilityData abilityData)
        {
            RemoveAbilityView();
            _activeAbilityData = abilityData;
            _iconComponent.sprite = abilityData.View.Icon;
            _button.interactable = true;
        }

        public void AssignPassiveAbilityData(IPassiveAbilityData abilityData)
        {
            RemoveAbilityView();
            _passiveAbilityData = abilityData;
            _iconComponent.sprite = abilityData.View.Icon;
            _button.interactable = true;
        }

        public void AssignPassiveAbilityRunner(PassiveAbilityRunner runner)
        {
            AssignPassiveAbilityData(runner.AbilityData);
            _passiveAbilityRunner = runner;
        }

        public void RemoveAbilityView()
        {
            _iconComponent.sprite = NoIconAvailableSprite;
            _button.interactable = false;
            _activeAbilityData = null;
            _passiveAbilityData = null;
            _passiveAbilityRunner = null;
        }
    } 
}
