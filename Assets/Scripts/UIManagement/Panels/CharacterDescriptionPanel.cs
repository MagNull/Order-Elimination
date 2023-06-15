using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using Inventory_Items;
using OrderElimination.AbilitySystem;
using OrderElimination;
using UnityEngine.Serialization;
using OrderElimination.MetaGame;
using OrderElimination.Localization;
using OrderElimination.Infrastructure;
using RoguelikeMap.UI.Abilities;
using Sirenix.OdinInspector;

namespace UIManagement
{
    public class CharacterDescriptionPanel : UIPanel
    {
        private static Dictionary<BattleStat, int> _statsElementsIdMapping = new()
        {
            { BattleStat.MaxHealth, 0 },
            { BattleStat.AttackDamage, 1 },
            { BattleStat.MaxArmor, 2 },
            { BattleStat.Evasion, 3 },
            { BattleStat.Accuracy, 4 },
        };

        public override PanelType PanelType => PanelType.CharacterDescription;
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        [SerializeField]
        private Sprite _noAbilityIcon;
        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI _characterName;
        [SerializeField]
        private Image _characterAvatar;
        [SerializeField]
        private List<IconTextValueElement> _characterStats;
        [SerializeReference]
        private List<Button> _activeAbilityButtons;
        [SerializeReference]
        private List<Button> _passiveAbilityButtons;
        [FormerlySerializedAs("_inventoryPresenter")]
        [SerializeField]
        private PickItemInventoryPresenter _playerInventoryPresenter;
        [SerializeField]
        private InventoryPresenter _characterInventoryPresenter;

        protected GameCharacter _characterData;

        protected GameCharacter _currentCharacterInfo;
        protected BattleCharacterView _currentBattleCharacterInfo;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateCharacterDescription(GameCharacter character)
        {
            if (character == null)
                Logging.LogException( new System.ArgumentNullException());
            _characterData = character;
            _characterName.text = character.CharacterData.Name;
            _characterAvatar.sprite = character.CharacterData.Avatar;
            UpdateBattleStats(character.CharacterStats);
            var activeAbilities = character
                .ActiveAbilities
                .Where(a => !a.View.HideInCharacterDiscription)
                .ToArray();
            var passiveAbilities = character
                .PassiveAbilities
                .Where(a => !a.View.HideInCharacterDiscription)
                .ToArray();
            UpdateAbilityButtonsInfo(activeAbilities, passiveAbilities);
            //_characterInventoryPresenter.enabled = true;
            //_playerInventoryPresenter.UpdateTargetInventory(_currentCharacterInfo.Inventory);
            //_characterInventoryPresenter.InitInventoryModel(_currentCharacterInfo.Inventory);
        }

        private void UpdateBattleStats(IReadOnlyGameCharacterStats stats)
        {
            if (_characterStats.Count != 5)
                Logging.LogException( new System.InvalidOperationException());
            foreach (var stat in EnumExtensions.GetValues<BattleStat>())
            {
                if (_statsElementsIdMapping.ContainsKey(stat))
                {
                    var item = _characterStats[_statsElementsIdMapping[stat]];
                    item.Text = Localization.Current.GetBattleStatName(stat);
                    if (stat == BattleStat.Accuracy || stat == BattleStat.Evasion)
                        item.Value = $"{stats[stat] * 100}%";
                    else
                        item.Value = stats[stat].ToString();
                }
            }
        }

        private void UpdateAbilityButtonsInfo(
            IActiveAbilityData[] activeAbilities, 
            IPassiveAbilityData[] passiveAbilities)
        {
            foreach (var button in _activeAbilityButtons.Concat(_passiveAbilityButtons))
            {
                button.image.sprite = _noAbilityIcon;
                button.onClick.RemoveAllListeners();
            }
            var displayedActiveAbilities = activeAbilities
                .Where(a => !a.View.HideInCharacterDiscription)
                .ToArray();
            var displayedPassiveAbilities = passiveAbilities
                .Where(a => !a.View.HideInCharacterDiscription)
                .ToArray();
            if (displayedActiveAbilities.Length > _activeAbilityButtons.Count
                || displayedPassiveAbilities.Length > _passiveAbilityButtons.Count)
                Logging.LogException( new System.NotSupportedException("Abilities to display count is greater than can be shown."));
            for (var i = 0; i < displayedActiveAbilities.Length; i++)
            {
                var button = _activeAbilityButtons[i];
                var ability = displayedActiveAbilities[i];
                button.image.sprite = ability.View.Icon;
                button.onClick.AddListener(OnActiveAbilityClicked);

                void OnActiveAbilityClicked()
                {
                    Logging.Log("No." , Colorize.Gold, context: this);
                    //_abilityInfoPanel.InitializeInfo(ability);
                    //_abilityInfoPanel.Open();
                }
            }
            for (var i = 0; i < displayedPassiveAbilities.Length; i++)
            {
                var button = _passiveAbilityButtons[i];
                button.image.sprite = displayedPassiveAbilities[i].View.Icon;
                button.onClick.AddListener(OnPassiveAbilityClicked);
            }

            void OnPassiveAbilityClicked()
            {
                Logging.Log("No." , Colorize.Gold, context: this);
                //_passiveAbilityInfoPanel.InitializeInfo(displayedPassiveAbilities);
                //_passiveAbilityInfoPanel.Open();
            }
        }

        private void OnActiveAbilityButtonClicked(SmallAbilityButton button)
        {
            //var descriptionPanel = (AbilityDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.AbilityDescription);
            //var stats = _currentBattleCharacterInfo?.Model.Stats ?? _currentCharacterInfo.GetBattleStats();
            //descriptionPanel.UpdateAbilityDescription(button.AbilityInfo, stats);
        }

        private void OnPassiveAbilityButtonClicked(SmallAbilityButton button)
        {
            //var passiveSkillsPanel = (PassiveSkillDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.PassiveSkillsDescription);
            //passiveSkillsPanel.AssignPassiveSkillsDescription(_passiveAbilitiesButtons.Select(b => b.AbilityInfo).ToArray());
        }
    }
}