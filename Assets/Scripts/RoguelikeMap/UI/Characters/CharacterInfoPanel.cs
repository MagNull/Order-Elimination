using System.Collections.Generic;
using System.Linq;
using GameInventory;
using GameInventory.Views;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.MacroGame;
using RoguelikeMap.UI.Abilities;
using Sirenix.OdinInspector;
using TMPro;
using UIManagement;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI.Characters
{
    public class CharacterInfoPanel : Panel
    {
        [SerializeField]
        private AbilityDescriptionPanel _abilityDescriptionPanel;
        [SerializeField]
        private PassiveAbilityDescriptionPanel _passiveAbilityDescriptionPanel;
        [SerializeField] 
        private PickItemInventoryPresenter _playerInventoryPresenter;
        [SerializeField]
        private ItemFilterPlayerInventoryView _playerInventoryView;

        [SerializeField]
        private InventoryPresenter _characterInventoryPresenter;
        
        [Title("Character")]
        [SerializeField]
        private TMP_Text _characterName;
        [SerializeField]
        private Image _characterAvatar;
        
        [Title("Stats")]
        [SerializeField]
        private TMP_Text _hpText;
        [SerializeField] 
        private TMP_Text _damageText;
        [SerializeField]
        private TMP_Text _armorText;
        [SerializeField] 
        private TMP_Text _evasionText;
        [SerializeField] 
        private TMP_Text _accuracyText;

        [Title("Abilities")]
        [PreviewField]
        [SerializeField]
        private Sprite _noAbilityIcon;
        [SerializeField]
        private List<Button> _activeAbilityButtons = new ();
        [SerializeField]
        private List<Button> _passiveAbilityButtons = new();
        
        public void InitializeCharacterInfo(GameCharacter character)
        {
            _characterName.text = character.CharacterData.Name;
            _characterAvatar.sprite = character.CharacterData.Avatar;
            InitializeStatsText(
                character.CharacterStats.MaxHealth,
                character.CharacterStats.MaxArmor,
                character.CharacterStats.Attack,
                character.CharacterStats.Accuracy,
                character.CharacterStats.Evasion);
            InitializeAbilityButtons(
                character.ActiveAbilities, 
                character.PassiveAbilities,
                ValueCalculationContext.ForMetaCaster(character));
            if (_playerInventoryPresenter is not null)
                _characterInventoryPresenter.InitInventoryModel(character.Inventory);
            _playerInventoryPresenter?.UpdateTargetInventory(character.Inventory);
            _playerInventoryView.UpdateCharacterData(character.CharacterData);
        }

        private void InitializeStatsText(
            float maxHealth, float maxArmor, float attack, float accuracy, float evasion)
        {
            _hpText.text = maxHealth.ToString();
            _armorText.text = maxArmor.ToString();
            _damageText.text = attack.ToString();
            _accuracyText.text = $"{accuracy * 100}%";
            _evasionText.text = $"{evasion * 100}%";
        }

        private void InitializeAbilityButtons(
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities,
            ValueCalculationContext calculationContext)
        {
            foreach (var button in _activeAbilityButtons.Concat(_passiveAbilityButtons))
            {
                button.image.sprite = _noAbilityIcon;
                button.onClick.RemoveAllListeners();
            }
            var unhiddenActiveAbilities = activeAbilities
                .Where(a => !a.View.HideInCharacterDescription)
                .ToArray();
            var unhiddenPassiveAbilities = passiveAbilities
                .Where(a => !a.View.HideInCharacterDescription)
                .ToArray();
            if (unhiddenActiveAbilities.Length > _activeAbilityButtons.Count
                || unhiddenPassiveAbilities.Length > _passiveAbilityButtons.Count)
                Logging.LogException(new System.NotSupportedException("Abilities to display count is greater than can be shown."));
            var activeAbilitiesToDisplayCount = Mathf.Min(_activeAbilityButtons.Count, unhiddenActiveAbilities.Length);
            var passiveAbilitiesToDisplayCount = Mathf.Min(_passiveAbilityButtons.Count, unhiddenPassiveAbilities.Length);
            for (var i = 0; i < activeAbilitiesToDisplayCount; i++)
            {
                var button = _activeAbilityButtons[i];
                var ability = unhiddenActiveAbilities[i];
                button.image.sprite = ability.View.Icon;
                button.onClick.AddListener(OnActiveAbilityClicked);

                void OnActiveAbilityClicked()
                {
                    _abilityDescriptionPanel.UpdateAbilityData(ability, calculationContext);
                    _abilityDescriptionPanel.Open();
                }
            }
            for (var i = 0; i < passiveAbilitiesToDisplayCount; i++)
            {
                var button = _passiveAbilityButtons[i];
                button.image.sprite = unhiddenPassiveAbilities[i].View.Icon;
                button.onClick.AddListener(OnPassiveAbilityClicked);
            }

            void OnPassiveAbilityClicked()
            {
                _passiveAbilityDescriptionPanel.UpdateAbilitiesDescription(unhiddenPassiveAbilities);
                _passiveAbilityDescriptionPanel.Open();
            }
        }
    }
}