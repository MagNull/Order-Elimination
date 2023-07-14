using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using GameInventory;
using OrderElimination.AbilitySystem;
using OrderElimination;
using UnityEngine.Serialization;
using OrderElimination.MacroGame;
using OrderElimination.Localization;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine.TextCore.Text;

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

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateCharacterDescription(GameCharacter character)
        {
            if (character == null)
                Logging.LogException( new System.ArgumentNullException());
            _characterName.text = character.CharacterData.Name;
            _characterAvatar.sprite = character.CharacterData.Avatar;
            UpdateBattleStats(character.CharacterStats);
            var activeAbilities = character
                .ActiveAbilities
                .Where(a => !a.View.HideInCharacterDescription)
                .ToArray();
            var passiveAbilities = character
                .PassiveAbilities
                .Where(a => !a.View.HideInCharacterDiscription)
                .ToArray();
            UpdateAbilityButtons(activeAbilities, passiveAbilities);
            UpdateInventory(character.Inventory);
        }

        public void UpdateCharacterDescription(AbilitySystemActor entity)
        {
            if (entity == null)
                Logging.LogException(new System.ArgumentNullException());
            var entitiesBank = entity.BattleContext.EntitiesBank;
            var gamecharacter = entitiesBank.GetBasedCharacter(entity);
            _characterName.text = gamecharacter.CharacterData.Name;
            _characterAvatar.sprite = gamecharacter.CharacterData.Avatar;
            UpdateBattleStats(entity.BattleStats);
            var activeAbilities = entity
                .ActiveAbilities
                .Select(runner => runner.AbilityData)
                .Where(a => !a.View.HideInCharacterDescription)
                .ToArray();
            var passiveAbilities = entity
                .PassiveAbilities
                .Select(runner => runner.AbilityData)
                .Where(a => !a.View.HideInCharacterDiscription)
                .ToArray();
            UpdateAbilityButtons(activeAbilities, passiveAbilities);
            _characterInventoryPresenter.enabled = false;
            //UpdateInventory();
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

        private void UpdateBattleStats(IBattleStats stats)
        {
            if (_characterStats.Count != 5)
                Logging.LogException(new System.InvalidOperationException());
            foreach (var stat in EnumExtensions.GetValues<BattleStat>())
            {
                if (_statsElementsIdMapping.ContainsKey(stat))
                {
                    var item = _characterStats[_statsElementsIdMapping[stat]];
                    item.Text = Localization.Current.GetBattleStatName(stat);
                    if (stat == BattleStat.Accuracy || stat == BattleStat.Evasion)
                        item.Value = $"{stats[stat].ModifiedValue * 100}%";
                    else
                        item.Value = stats[stat].ModifiedValue.ToString();
                }
            }
        }

        private void UpdateAbilityButtons(
            IActiveAbilityData[] activeAbilities, 
            IPassiveAbilityData[] passiveAbilities)
        {
            foreach (var button in _activeAbilityButtons.Concat(_passiveAbilityButtons))
            {
                button.image.sprite = _noAbilityIcon;
                button.onClick.RemoveAllListeners();
            }
            var displayedActiveAbilities = activeAbilities
                .Where(a => !a.View.HideInCharacterDescription)
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
                    var panel = (AbilityDescriptionPanel)
                    UIController.SceneInstance.OpenPanel(PanelType.AbilityDescription);
                    panel.UpdateAbilityData(ability);
                    panel.Open();
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
                var panel = (PassiveAbilityDescriptionPanel)
                    UIController.SceneInstance.OpenPanel(PanelType.PassiveAbilityDescription);
                panel.UpdateAbilitiesDescription(displayedPassiveAbilities);
                panel.Open();
            }
        }

        private void UpdateInventory(Inventory inventory)
        {
            _characterInventoryPresenter.enabled = true;
            _playerInventoryPresenter.UpdateTargetInventory(inventory);
            _characterInventoryPresenter.InitInventoryModel(inventory);
        }
    }
}