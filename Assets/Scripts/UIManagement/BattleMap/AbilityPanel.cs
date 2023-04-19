using CharacterAbility;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private SmallAbilityButton[] _passiveAbilityButtons;
        [SerializeField]
        private UIController _panelController;
        [SerializeField]
        private Sprite _noSelectedAbilityIcon;
        [SerializeField]
        private Color _selectedAbilityTint;
        private AbilityView[] _currentPassiveSkills;

        public bool AbilityCasing => _activeAbilityButtons.Any(ab => ab.AbilityView is {Casting: true} &&
                                                                     ab.AbilityView.AbilityInfo.ActionType !=
                                                                     ActionType.Movement);

        public IReadOnlyList<AbilityButton> AbilityButtons => _activeAbilityButtons;

        private void OnEnable()
        {
            foreach (var abilityButton in _activeAbilityButtons)
            {
                abilityButton.Clicked += OnActiveAbilityButtonClicked;
                abilityButton.Holded += OnActiveAbilityButtonHolded;
                abilityButton.AbilityButtonUsed += UpdateActiveAbilityButtonsAvailability;
                abilityButton.NoSelectedAbilityIcon = _noSelectedAbilityIcon;
            }

            foreach (var abilityButton in _passiveAbilityButtons)
            {
                abilityButton.Clicked += OnPassiveAbilityButtonClicked;
                abilityButton.NoIconAvailableSprite = _noSelectedAbilityIcon;
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

            foreach (var skillButton in _passiveAbilityButtons)
            {
                skillButton.Clicked -= OnPassiveAbilityButtonClicked;
            }
        }

        public void SelectFirstAvailableAbility()
        {
            var firstAvailableAbilityButton = _activeAbilityButtons.FirstOrDefault(b => b.AbilityView.CanCast);
            if (firstAvailableAbilityButton != null)
            {
                OnActiveAbilityButtonClicked(firstAvailableAbilityButton);
                firstAvailableAbilityButton.Select();
            }
        }

        public void old_AssignAbilities(AbilityView[] activeAbilitiesView, AbilityView[] passiveAbilitiesView)
        {
            if (activeAbilitiesView.Length > _activeAbilityButtons.Length
                || passiveAbilitiesView.Length > _passiveAbilityButtons.Length)
                throw new ArgumentException();
            ResetAbilityButtons();
            for (var i = 0; i < activeAbilitiesView.Length; i++)
            {
                _activeAbilityButtons[i].CancelAbilityCast();
                _activeAbilityButtons[i].old_AssignAbilityView(activeAbilitiesView[i]);
                _activeAbilityButtons[i].HoldableButton.HoldAvailable = true;
            }

            for (var j = 0; j < passiveAbilitiesView.Length; j++)
            {
                _passiveAbilityButtons[j].AssignAbilityView(passiveAbilitiesView[j].AbilityInfo);
            }

            _currentPassiveSkills = passiveAbilitiesView;
        }

        public void AssignAbilities(
            OrderElimination.AbilitySystem.AbilityRunner[] activeAbilities,
            OrderElimination.AbilitySystem.AbilityRunner[] passiveAbilities)
        {
            if (activeAbilities.Length > _activeAbilityButtons.Length
                || passiveAbilities.Length > _passiveAbilityButtons.Length)
                throw new ArgumentException();
            ResetAbilityButtons();
            for (var i = 0; i < activeAbilities.Length; i++)
            {
                _activeAbilityButtons[i].CancelAbilityCast();
                _activeAbilityButtons[i].AssignAbiility(activeAbilities[i]);
                _activeAbilityButtons[i].HoldableButton.HoldAvailable = true;
            }

            for (var j = 0; j < passiveAbilities.Length; j++)
            {
                //_passiveAbilityButtons[j].AssignAbilityView(passiveAbilities[j]);
            }
        }

        public void ResetAbilityButtons()
        {
            foreach (var abilityButton in _activeAbilityButtons)
            {
                abilityButton.RemoveAbilityView();
                abilityButton.HoldableButton.ClickAvailable = true;
            }

            foreach (var button in _passiveAbilityButtons)
            {
                button.RemoveAbilityView();
            }

            _currentPassiveSkills = null;
        }

        private void OnActiveAbilityButtonClicked(AbilityButton abilityButton)
        {
            foreach (var button in _activeAbilityButtons)
            {
                button.HoldableButton.SetImageTint(Color.white);
                if (button == abilityButton)
                {
                    var tint = button.AbilityView.Casting
                        ? Color.white
                        : _selectedAbilityTint;
                    button.HoldableButton.SetImageTint(tint);

                }
                else
                    button.CancelAbilityCast();
            }
        }

        private void OnActiveAbilityButtonHolded(AbilityButton abilityButton)
        {
            var panel = (AbilityDescriptionPanel) _panelController.OpenPanel(PanelType.AbilityDescription);
            panel.UpdateAbilityDescription(abilityButton.AbilityView);
        }

        private void UpdateActiveAbilityButtonsAvailability()
        {
            foreach (var button in _activeAbilityButtons)
            {
                button.UpdateAvailability();
            }
        }

        private void OnPassiveAbilityButtonClicked(SmallAbilityButton skillButton)
        {
            var panel = _panelController.OpenPanel(PanelType.PassiveSkillsDescription) as PassiveSkillDescriptionPanel;
            panel.AssignPassiveSkillsDescription(_currentPassiveSkills.Select(v => v.AbilityInfo).ToArray());
        }
    }
}