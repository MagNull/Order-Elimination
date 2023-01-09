using CharacterAbility;
using System;
using System.Collections.Generic;
using UIManagement;
using UIManagement.Elements;
using UnityEngine;

namespace UIManagement
{
    //TODO(САНО): Refactor
    public class AbilityPanel : MonoBehaviour
    {
        [SerializeField]
        private AbilityButton[] _activeAbilityButtons;
        [SerializeField]
        private SmallAbilityButton[] _passiveSkillButtons;
        [SerializeField]
        private UIController _panelController;

        private Dictionary<AbilityView, AbilityButton> _buttonsByView = new Dictionary<AbilityView, AbilityButton>();
        private AbilityView[] _currentPassiveSkills;

        public IReadOnlyList<AbilityButton> AbilityButtons => _activeAbilityButtons;

        private void OnEnable()
        {
            foreach(var abilityButton in _activeAbilityButtons)
            {
                abilityButton.Clicked += OnActiveAbilityButtonClicked;
                abilityButton.Holded += OnActiveAbilityButtonHolded;
                abilityButton.AbilityButtonUsed += UpdateActiveAbilityButtonsAvailability;
            }
            foreach (var skillButton in _passiveSkillButtons)
            {
                skillButton.Clicked += OnPassiveSkillButtonClicked;
            }
        }

        private void OnDisable()
        {
            foreach (var button in _activeAbilityButtons)
            {
                button.Clicked -= OnActiveAbilityButtonClicked;
                button.Holded -= OnActiveAbilityButtonHolded;
                button.AbilityButtonUsed -= UpdateActiveAbilityButtonsAvailability;
            }
            foreach (var skillButton in _passiveSkillButtons)
            {
                skillButton.Clicked -= OnPassiveSkillButtonClicked;
            }
        }

        public void Select(AbilityView abilityView)
        {
            if (!_buttonsByView.ContainsKey(abilityView))
                throw new KeyNotFoundException($"No ability \"{abilityView.Name}\" assigned.");
            _buttonsByView[abilityView].Select();
        }

        public void AssignAbilities(AbilityView[] activeAbilitiesView, AbilityView[] passiveAbilitiesView)
        {
            if (activeAbilitiesView.Length > _activeAbilityButtons.Length
                || passiveAbilitiesView.Length > _passiveSkillButtons.Length)
                throw new ArgumentException();
            ResetAbilityButtons();
            for (var i = 0; i < activeAbilitiesView.Length; i++)
            {
                _activeAbilityButtons[i].CancelAbilityCast();
                _activeAbilityButtons[i].AssignAbilityView(activeAbilitiesView[i]);
                _activeAbilityButtons[i].HoldableButton.HoldAvailable = true;
                _buttonsByView.Add(activeAbilitiesView[i], _activeAbilityButtons[i]);
            }
            for (var j = 0; j < passiveAbilitiesView.Length; j++)
            {
                _passiveSkillButtons[j].AssignAbilityView(passiveAbilitiesView[j]);
            }
            _currentPassiveSkills = passiveAbilitiesView;
        }
        public void ResetAbilityButtons()
        {
            foreach (var abilityButton in _activeAbilityButtons)
            {
                abilityButton.RemoveAbilityView();
                abilityButton.HoldableButton.ClickAvailable = true;
            }
            foreach (var button in _passiveSkillButtons)
            {
                button.RemoveAbilityView();
            }
            _buttonsByView.Clear();
            _currentPassiveSkills = null;
        }

        private void OnActiveAbilityButtonClicked(AbilityButton abilityButton)
        {
            foreach (var button in _activeAbilityButtons)
            {
                if(button != abilityButton)
                {
                    button.CancelAbilityCast();
                }
            }
        }

        private void OnActiveAbilityButtonHolded(AbilityButton abilityButton)
        {
            var panel = (AbilityDescriptionPanel)_panelController.OpenPanel(PanelType.AbilityDescription);
            panel.UpdateAbilityDescription(abilityButton.AbilityView);
        }

        private void UpdateActiveAbilityButtonsAvailability()
        {
            foreach(var button in _activeAbilityButtons)
            {
                button.UpdateAvailability();
            }
        }

        private void OnPassiveSkillButtonClicked(SmallAbilityButton skillButton)
        {
            var panel = _panelController.OpenPanel(PanelType.PassiveSkillsDescription) as PassiveSkillDescriptionPanel;
            panel.AssignPassiveSkillsDescription(_currentPassiveSkills);
        }
    }
}