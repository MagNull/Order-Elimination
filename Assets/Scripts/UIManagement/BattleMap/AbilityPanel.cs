using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UIManagement.Elements;
using UnityEngine;

namespace UIManagement
{
    public class AbilityPanel : MonoBehaviour
    {
        [SerializeField]
        private AbilityButton[] _activeAbilityButtons;

        [SerializeField]
        private AbilityButton _itemAbilityButton;

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
            foreach (var abilityButton in _activeAbilityButtons.Append(_itemAbilityButton))
            {
                abilityButton.Clicked += OnAbilityButtonClicked;
                abilityButton.Holded += OnActiveAbilityButtonHolded;
                abilityButton.NoSelectedAbilityIcon = _noSelectedAbilityIcon;
            }
        }

        private void OnDisable()
        {
            foreach (var button in _activeAbilityButtons.Append(_itemAbilityButton))
            {
                button.Clicked -= OnAbilityButtonClicked;
                button.Holded -= OnActiveAbilityButtonHolded;
            }
        }

        public void AssignAbilities(
            AbilitySystemActor caster,
            ActiveAbilityRunner[] activeAbilities)
        {
            if (activeAbilities.Length > _activeAbilityButtons.Append(_itemAbilityButton).Count())
                Logging.LogException(new ArgumentException());
            ResetAbilityButtons();
            _caster = caster;
            _battleContext = caster.BattleContext;
            for (var i = 0; i < activeAbilities.Length; i++)
            {
                if (activeAbilities[i].AbilityProvider == AbilityProvider.Equipment)
                {
                    _itemAbilityButton.AssignAbiility(activeAbilities[i]);
                    _itemAbilityButton.HoldableButton.ClickAvailable = true;
                    _itemAbilityButton.HoldableButton.HoldAvailable = true;
                }
                else
                {
                    _activeAbilityButtons[i].AssignAbiility(activeAbilities[i]);
                    _activeAbilityButtons[i].HoldableButton.ClickAvailable = true;
                    _activeAbilityButtons[i].HoldableButton.HoldAvailable = true;
                }
            }

            UpdateAbilityButtonsAvailability();
        }

        public void ResetAbilityButtons()
        {
            foreach (var abilityButton in _activeAbilityButtons.Append(_itemAbilityButton))
            {
                if (abilityButton.AbilityRunner != null)
                {
                    TryDeselectButton(abilityButton);
                }

                abilityButton.RemoveAbility();
                abilityButton.HoldableButton.ClickAvailable = false;
                abilityButton.HoldableButton.HoldAvailable = false;
            }
        }

        public void UpdateAbilityButtonsAvailability()
        {
            foreach (var button in _activeAbilityButtons.Append(_itemAbilityButton))
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
            foreach (var button in _activeAbilityButtons.Append(_itemAbilityButton))
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
            foreach (var otherButton in _activeAbilityButtons.Where(b => b != abilityButton).Append(_itemAbilityButton))
            {
                TryDeselectButton(otherButton);
            }

            if (TryDeselectButton(abilityButton))
            {
            }
            else if (TrySelectButton(abilityButton))
            {
            }
            else
            {
            }
        }

        private void OnActiveAbilityButtonHolded(AbilityButton abilityButton)
        {
            //var panel = (AbilityDescriptionPanel) _panelController.OpenPanel(PanelType.AbilityDescription);
            //panel.UpdateAbilityDescription(abilityButton.AbilityView);
        }

        private void OnPassiveAbilityButtonClicked(SmallAbilityButton skillButton)
        {
            var panel =
                _panelController.OpenPanel(PanelType.PassiveSkillsDescription) as PassiveAbilityDescriptionPanel;
            //panel.AssignPassiveSkillsDescription(_currentPassiveSkills.Select(v => v.AbilityInfo).ToArray());
        }
    }
}