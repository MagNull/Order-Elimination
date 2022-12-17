﻿using System;
using System.Collections.Generic;
using UIManagement;
using UnityEngine;

namespace CharacterAbility
{
    //TODO(САНО): Refactor
    public class AbilityPanel : MonoBehaviour
    {
        [SerializeField]
        private AbilityButton[] _abilityButtons;
        [SerializeField]
        private AbilityButton[] _passiveSkillsButtons;
        [SerializeField]
        private UIController _panelController;
        public IReadOnlyList<AbilityButton> AbilityButtons => _abilityButtons;

        public void ResetAbilityButtons()
        {
            foreach (var button in _abilityButtons)
            {
                button.RemoveAbility();
            }    
        }
        
        private void OnEnable()
        {
            foreach(var button in _abilityButtons)
            {
                button.Clicked += OnAbilityButtonClicked;
                button.Holded += OnAbilityButtonHolded;
                button.AbilityButtonUsed += OnAbilityCasted;
            }
        }

        private void OnDisable()
        {
            foreach (var button in _abilityButtons)
            {
                button.Clicked -= OnAbilityButtonClicked;
                button.Holded -= OnAbilityButtonHolded;
                button.AbilityButtonUsed -= OnAbilityCasted;
            }
        }

        private void OnAbilityButtonClicked(AbilityButton abilityButton)
        {
            foreach (var button in _abilityButtons)
            {
                if(button != abilityButton)
                {
                    button.CancelAbilityCast();
                }
            }
        }

        private void OnAbilityButtonHolded(AbilityButton abilityButton)
        {
            var panel = (AbilityDescriptionPanel)_panelController.OpenPanel(PanelType.AbilityDescription);
            panel.UpdateAbilityDescription(abilityButton.AbilityView);
        }

        private void OnAbilityCasted()
        {
            foreach(var button in _abilityButtons)
            {
                //button.CheckUsePossibility();
            }
        }
    }
}