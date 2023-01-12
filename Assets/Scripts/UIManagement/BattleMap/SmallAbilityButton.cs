using CharacterAbility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [RequireComponent(typeof(HoldableButton))]
    public class SmallAbilityButton : MonoBehaviour
    {
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
        public AbilityView AbilityView { get; private set; }
        public event Action<SmallAbilityButton> Clicked;

        private void Awake()
        {
            _button.ClickAvailable = true;
            _button.HoldAvailable = false;
            _button.interactable = false;
            _button.Clicked += OnClick;
        }

        private void OnClick(HoldableButton button) => Clicked?.Invoke(this);

        public void AssignAbilityView(AbilityView abilityView)
        {
            AbilityView = abilityView;
            _iconComponent.sprite = AbilityView.AbilityIcon;
            _button.interactable = true;
        }

        public void RemoveAbilityView()
        {
            AbilityView = null;
            _iconComponent.sprite = NoIconAvailableSprite;
            _button.interactable = false;
        }
    } 
}
