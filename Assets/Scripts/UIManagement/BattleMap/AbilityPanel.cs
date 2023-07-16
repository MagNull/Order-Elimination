using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UIManagement.Elements;
using UnityEngine;
using DG.Tweening;

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
                var ability = activeAbilities[i];
                if (activeAbilities[i].AbilityProvider == AbilityProvider.Equipment)
                {
                    _itemAbilityButton.AssignAbiility(ability);
                    _itemAbilityButton.HoldableButton.ClickAvailable = true;
                    _itemAbilityButton.HoldableButton.HoldAvailable = true;
                }
                else
                {
                    _activeAbilityButtons[i].AssignAbiility(ability);
                    _activeAbilityButtons[i].HoldableButton.ClickAvailable = true;
                    _activeAbilityButtons[i].HoldableButton.HoldAvailable = true;
                    _activeAbilityButtons[i].CooldownTimer.SetValue(CalculateCooldown(ability, _battleContext), 0);
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

                var ability = button.AbilityRunner;
                var isAvailable = ability.IsCastAvailable(_battleContext, _caster);
                button.CooldownTimer.SetValue(CalculateCooldown(ability, _battleContext));
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

            if (!TryDeselectButton(abilityButton))
                TrySelectButton(abilityButton);
        }

        private int CalculateCooldown(ActiveAbilityRunner abilityRunner, IBattleContext battleContext)
        {
            var cooldownTime = abilityRunner.Cooldown;
            var currentRound = battleContext.CurrentRound;
            var cooldownValueToDisplay = cooldownTime;
            var roundConditions = abilityRunner.AbilityData.Rules.AvailabilityConditions
                .Select(c => c as UnlocksAtRoundCondition)
                .Where(c => c != null)
                .ToArray();
            if (roundConditions.Length > 0)
            {
                var unlockingRound = roundConditions.Max(c => c.UnlocksAtRound);
                if (currentRound < unlockingRound)
                {
                    cooldownValueToDisplay = Mathf.Max(cooldownTime, unlockingRound - currentRound);
                }
            }
            return cooldownValueToDisplay;
        }

        private void OnActiveAbilityButtonHolded(AbilityButton abilityButton)
        {
            var panel = (AbilityDescriptionPanel)_panelController.OpenPanel(PanelType.AbilityDescription);
            panel.UpdateAbilityData(abilityButton.AbilityRunner.AbilityData);
        }

        private void OnPassiveAbilityButtonClicked(SmallAbilityButton skillButton)
        {
            //var panel = (PassiveAbilityDescriptionPanel)_panelController.OpenPanel(PanelType.PassiveAbilityDescription);
            //panel.UpdateAbilitiesDescription(_currentPassiveSkills.Select(v => v.AbilityInfo).ToArray());
        }
    }
}