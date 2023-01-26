using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using Sirenix.OdinInspector;
using OrderElimination;
using System;
using DefaultNamespace;
using UIManagement.trashToRemove_Mockups;

namespace UIManagement
{
    public class CharacterUpgradePanel : CharacterDescriptionPanel
    {
        public override PanelType PanelType => PanelType.CharacterUpgradable;
        [SerializeField] private HoldableButton _upgradeButton;
        [SerializeField] private TextMeshProUGUI _upgradeText;
        [SerializeField] private TextMeshProUGUI _upgradeCost;
        [SerializeField] private TextEmitter _textEmitter;
        private CharacterUpgradeTransaction _currentUpgradeTransaction;

        public void UpdateCharacterUpgradeDescription(CharacterUpgradeTransaction transaction)
        {
            base.UpdateCharacterDescription(transaction.TargetCharacter);
            _currentUpgradeTransaction = transaction;
            _upgradeCost.text = transaction.Cost.ToString();
            _upgradeButton.Clicked -= OnUpgradeButtonClicked;
            _upgradeButton.Clicked += OnUpgradeButtonClicked;
        }

        private void OnUpgradeButtonClicked(HoldableButton button)
        {
            var upgradeResult = _currentUpgradeTransaction.TryUpgrade();
            if(upgradeResult)
            {
                _textEmitter.Emit($"-{_currentUpgradeTransaction.Cost}", Color.yellow);
                UpdateCharacterDescription(_currentCharacterInfo);
            }
        }
    }
}