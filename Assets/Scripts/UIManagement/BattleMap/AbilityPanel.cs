using CharacterAbility;
using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UIManagement;
using UIManagement.Elements;
using UnityEngine;
using VContainer;

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
        private IBattleContext _battleContext;
        private AbilitySystemActor _caster;
        private HashSet<AbilityButton> _selectedButtons = new();

        public event Action<ActiveAbilityRunner> AbilitySelected;
        public event Action<ActiveAbilityRunner> AbilityDeselected;

        private void OnEnable()
        {
            foreach (var abilityButton in _activeAbilityButtons)
            {
                abilityButton.Clicked += OnAbilityButtonClicked;
                abilityButton.Holded += OnActiveAbilityButtonHolded;
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
                button.Clicked -= OnAbilityButtonClicked;
                button.Holded -= OnActiveAbilityButtonHolded;
            }

            foreach (var skillButton in _passiveAbilityButtons)
            {
                skillButton.Clicked -= OnPassiveAbilityButtonClicked;
            }
        }

        public void AssignAbilities(
            AbilitySystemActor caster,
            ActiveAbilityRunner[] activeAbilities,
            PassiveAbilityRunner[] passiveAbilities)
        {
            if (activeAbilities.Length > _activeAbilityButtons.Length
                || passiveAbilities.Length > _passiveAbilityButtons.Length)
                throw new ArgumentException();
            ResetAbilityButtons();
            _caster = caster;
            _battleContext = caster.BattleContext;
            for (var i = 0; i < activeAbilities.Length; i++)
            {
                _activeAbilityButtons[i].AssignAbiility(activeAbilities[i]);
                _activeAbilityButtons[i].HoldableButton.ClickAvailable = true;
                _activeAbilityButtons[i].HoldableButton.HoldAvailable = true;
            }
            UpdateAbilityButtonsAvailability();
            for (var j = 0; j < passiveAbilities.Length; j++)
            {
                _passiveAbilityButtons[j].AssignPassiveAbilityRunner(passiveAbilities[j]);
            }
        }

        public void ResetAbilityButtons()
        {
            foreach (var abilityButton in _activeAbilityButtons)
            {
                if (abilityButton.AbilityRunner != null)
                {
                    TryDeselectButton(abilityButton);
                }
                abilityButton.RemoveAbility();
                abilityButton.HoldableButton.ClickAvailable = false;
                abilityButton.HoldableButton.HoldAvailable = false;
            }

            foreach (var button in _passiveAbilityButtons)
            {
                button.RemoveAbilityView();
            }
        }

        public void UpdateAbilityButtonsAvailability()
        {
            foreach (var button in _activeAbilityButtons)
            {
                if (button.AbilityRunner == null)
                {
                    button.SetClickAvailability(false);
                    continue;
                }
                var isAvailable = button.AbilityRunner.IsCastAvailable(_battleContext, _caster);
                button.SetClickAvailability(isAvailable);
            }
        }

        private void OnAbilityUsed(ActiveAbilityRunner ability)
        {
            foreach (var button in _activeAbilityButtons)
                TryDeselectButton(button);
            UpdateAbilityButtonsAvailability();
        }

        private bool TrySelectButton(AbilityButton abilityButton)
        {
            if (_selectedButtons.Contains(abilityButton))
                return false;
            abilityButton.AbilityRunner.AbilityExecutionStarted -= OnAbilityUsed;
            abilityButton.AbilityRunner.AbilityExecutionStarted += OnAbilityUsed;
            _selectedButtons.Add(abilityButton);
            abilityButton.HoldableButton.SetImageTint(_selectedAbilityTint);
            AbilitySelected?.Invoke(abilityButton.AbilityRunner);
            return true;
        }

        private bool TryDeselectButton(AbilityButton abilityButton)
        {
            if (!_selectedButtons.Contains(abilityButton))
                return false;
            abilityButton.AbilityRunner.AbilityExecutionStarted -= OnAbilityUsed;
            _selectedButtons.Remove(abilityButton);
            abilityButton.HoldableButton.SetImageTint(Color.white);
            AbilityDeselected?.Invoke(abilityButton.AbilityRunner);
            return true;
        }

        private void OnAbilityButtonClicked(AbilityButton abilityButton)
        {
            foreach (var otherButton in _activeAbilityButtons.Where(b => b != abilityButton))
            {
                TryDeselectButton(otherButton);
            }
            if (TryDeselectButton(abilityButton)) { }
            else if (TrySelectButton(abilityButton)) { }
            else { }
        }

        private void OnActiveAbilityButtonHolded(AbilityButton abilityButton)
        {
            //var panel = (AbilityDescriptionPanel) _panelController.OpenPanel(PanelType.AbilityDescription);
            //panel.UpdateAbilityDescription(abilityButton.AbilityView);
        }

        private void OnPassiveAbilityButtonClicked(SmallAbilityButton skillButton)
        {
            var panel = _panelController.OpenPanel(PanelType.PassiveSkillsDescription) as PassiveSkillDescriptionPanel;
            //panel.AssignPassiveSkillsDescription(_currentPassiveSkills.Select(v => v.AbilityInfo).ToArray());
        }
    }
}