using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using GameInventory;
using OrderElimination.AbilitySystem;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.Localization;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;

namespace UIManagement
{
    public class CharacterDescriptionPanel : UIPanel
    {
        private static readonly Dictionary<BattleStat, int> _statsElementsIdMapping = new()
        {
            { BattleStat.MaxHealth, 0 },
            { BattleStat.AttackDamage, 1 },
            { BattleStat.MaxArmor, 2 },
            { BattleStat.Evasion, 3 },
            { BattleStat.Accuracy, 4 },
        };

        public override PanelType PanelType => PanelType.CharacterDescription;

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
        [SerializeField]
        private InventoryPresenter _characterInventoryPresenter;

        [Header("Parameters")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        [SerializeField]
        private Sprite _noAbilityIcon;

        [SerializeField]
        private bool _roundBattleStats;

        [ShowIf(nameof(_roundBattleStats))]
        [SerializeField]
        private RoundingOption _roundingMode = RoundingOption.Math;

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
                .Where(a => !a.View.HideInCharacterDescription)
                .ToArray();
            UpdateAbilityButtons(
                activeAbilities, passiveAbilities, ValueCalculationContext.ForMetaCaster(character));
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
            UpdateBattleStats(entity.BattleStats, gamecharacter.CharacterStats);
            var activeAbilities = entity
                .ActiveAbilities
                .Select(runner => runner.AbilityData)
                .Where(a => !a.View.HideInCharacterDescription)
                .ToArray();
            var passiveAbilities = entity
                .PassiveAbilities
                .Select(runner => runner.AbilityData)
                .Where(a => !a.View.HideInCharacterDescription)
                .ToArray();
            UpdateAbilityButtons(
                activeAbilities, passiveAbilities, ValueCalculationContext.ForBattleCaster(entity.BattleContext, entity));
            _characterInventoryPresenter.enabled = false;
            if (entity.EntityType == EntityType.Character)
            {
                var character = entity.BattleContext.EntitiesBank.GetBasedCharacter(entity);
                UpdateInventory(character.Inventory);
            }
        }

        private void UpdateBattleStats(IReadOnlyGameCharacterStats stats)
        {
            ResetStatsColor();
            if (_characterStats.Count != 5)
                Logging.LogException(new System.InvalidOperationException());
            EnumExtensions
                .GetValues<BattleStat>()
                .Where(stat => _statsElementsIdMapping.ContainsKey(stat))
                .ForEach(stat => UpdateStat(stat, stats[stat]));
        }

        private void UpdateBattleStats(IBattleStats stats, IReadOnlyGameCharacterStats defaultStats)
        {
            ResetStatsColor();
            if (_characterStats.Count != 5)
                Logging.LogException(new System.InvalidOperationException());
            foreach (var stat in EnumExtensions
                .GetValues<BattleStat>()
                .Where(stat => _statsElementsIdMapping.ContainsKey(stat)))
            {
                UpdateStat(stat, stats[stat].ModifiedValue);
                if (stats[stat].ModifiedValue > defaultStats[stat])
                    _characterStats[_statsElementsIdMapping[stat]].SetValueColor(Color.green);
                if (stats[stat].ModifiedValue < defaultStats[stat])
                    _characterStats[_statsElementsIdMapping[stat]].SetValueColor(Color.red);
            }
        }

        private void UpdateStat(BattleStat stat, float value)
        {
            var displayedStat = value;
            if (stat.IsPercentStat())
                displayedStat *= 100;
            if (_roundBattleStats)
                displayedStat = MathExtensions.Round(displayedStat, _roundingMode);
            var item = _characterStats[_statsElementsIdMapping[stat]];
            item.Text = Localization.Current.GetBattleStatName(stat);
            if (stat.IsPercentStat())
                item.Value = $"{displayedStat}%";
            else
                item.Value = $"{displayedStat}";
        }

        private void UpdateAbilityButtons(
            IActiveAbilityData[] activeAbilities, 
            IPassiveAbilityData[] passiveAbilities,
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
                    var panel = (AbilityDescriptionPanel)
                    UIController.SceneInstance.OpenPanel(PanelType.AbilityDescription);
                    panel.UpdateAbilityData(ability, calculationContext);
                    panel.Open();
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
                var panel = (PassiveAbilityDescriptionPanel)
                    UIController.SceneInstance.OpenPanel(PanelType.PassiveAbilityDescription);
                panel.UpdateAbilitiesDescription(unhiddenPassiveAbilities);
                panel.Open();
            }
        }

        private void UpdateInventory(Inventory inventory)
        {
            _characterInventoryPresenter.enabled = true;
            _characterInventoryPresenter.InitInventoryModel(inventory);
        }

        private void ResetStatsColor()
        {
            _characterStats.ForEach(stat => stat.SetValueColor(Color.white));
        }
    }
}