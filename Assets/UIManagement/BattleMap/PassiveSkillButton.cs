using CharacterAbility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [RequireComponent(typeof(HoldableButton))]
    public class PassiveSkillButton : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] HoldableButton _button;
        public AbilityView SkillView { get; private set; }
        public event Action<PassiveSkillButton> Clicked;

        private void Awake()
        {
            _button.ClickAvailable = true;
            _button.HoldAvailable = false;
            _button.interactable = false;
            _button.Clicked += OnClick;
        }

        private void OnClick(HoldableButton button) => Clicked?.Invoke(this);

        public void AssignPassiveSkill(AbilityView skillView)
        {
            _icon.sprite = skillView.AbilityIcon;
            _button.interactable = true;
        }

        public void RemoveSkillView()
        {
            _icon.sprite = null;
            SkillView = null;
            _button.interactable = false;
        }
    } 
}
