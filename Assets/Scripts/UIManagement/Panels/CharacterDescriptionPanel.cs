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
        [SerializeField]
        private TextMeshProUGUI _characterName;
        [SerializeField]
        private Image _characterAvatar;
        [SerializeField]
        private List<IconTextValueElement> _characterStats;
        [SerializeReference]
        private List<SmallAbilityButton> _activeAbilitiesButtons;
        [SerializeReference]
        private List<SmallAbilityButton> _passiveAbilitiesButtons;
        [SerializeReference]
        private List<AbilityInfo> _ignoredActiveAbilities;
        [FormerlySerializedAs("_inventoryPresenter")]
        [SerializeField]
        private PickItemInventoryPresenter _playerInventoryPresenter;
        [SerializeField]
        private InventoryPresenter _characterInventoryPresenter;

        protected GameCharacter _characterData;

        protected OrderElimination.Character _currentCharacterInfo;
        protected BattleCharacterView _currentBattleCharacterInfo;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateCharacterDescription(GameCharacter character)
        {
            _characterData = character;
            _characterName.text = character.CharacterData.Name;
            _characterAvatar.sprite = character.CharacterData.Avatar;
            UpdateBattleStats(character.CharacterStats);

        }

        public void UpdateCharacterDescription(BattleCharacterView characterView)
        {
            if (characterView == null)
            {
                throw new System.ArgumentException();
            }
            var battleStats = characterView.Model.Stats;
            var activeAbilities = characterView.ActiveAbilitiesView
                .Select(v => v.AbilityInfo)
                .Where(i => !_ignoredActiveAbilities.Contains(i))
                .ToArray();
            var passiveAbilities = characterView.PassiveAbilitiesView.Select(v => v.AbilityInfo).ToArray();
            _currentCharacterInfo = null;
            _currentBattleCharacterInfo = characterView;
            _characterAvatar.sprite = characterView.AvatarFull;
            _characterName.text = characterView.CharacterName;
            //UpdateBattleStats(battleStats);
            UpdateAbilityButtonsInfo(activeAbilities, passiveAbilities);
        }

        public void UpdateCharacterDescription(Character characterInfo)
        {
            if (characterInfo == null)
            {
                throw new System.ArgumentException();
            }
            var battleStats = characterInfo.GetBattleStats();
            var activeAbilities = characterInfo.GetActiveAbilityInfos().Where(i => !_ignoredActiveAbilities.Contains(i)).ToArray();
            var passiveAbilities = characterInfo.GetPassiveAbilityInfos();
            _currentCharacterInfo = characterInfo;
            _playerInventoryPresenter.UpdateTargetInventory(_currentCharacterInfo.Inventory);
            _characterInventoryPresenter.InitInventoryModel(_currentCharacterInfo.Inventory);
            _characterInventoryPresenter.enabled = true;
            _currentBattleCharacterInfo = null;
            _characterAvatar.sprite = characterInfo.Avatar;
            _characterName.text = characterInfo.Name;
            //UpdateBattleStats(battleStats);
            UpdateAbilityButtonsInfo(activeAbilities, passiveAbilities);
        }

        private void UpdateBattleStats(IReadOnlyGameCharacterStats stats)
        {
            if (_characterStats.Count != 5)
                throw new System.InvalidOperationException();
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

        private void UpdateAbilityButtonsInfo(AbilityInfo[] activeAbilities, AbilityInfo[] passiveAbilities)
        {
            foreach (var b in _activeAbilitiesButtons.Concat(_passiveAbilitiesButtons))
                b.RemoveAbilityView();
            if (activeAbilities.Length > _activeAbilitiesButtons.Count
                || passiveAbilities.Length > _passiveAbilitiesButtons.Count)
                throw new System.InvalidOperationException("Maximum of 4 active and 2 passive abilities is supported");
            for (var i = 0; i < activeAbilities.Length; i++)
            {
                //_activeAbilitiesButtons[i].AssignAbilityView(activeAbilities[i]);
                _activeAbilitiesButtons[i].Clicked -= OnActiveAbilityButtonClicked;
                _activeAbilitiesButtons[i].Clicked += OnActiveAbilityButtonClicked;
            }
            for (var j = 0; j < passiveAbilities.Length; j++)
            {
                //_passiveAbilitiesButtons[j].AssignAbilityView(passiveAbilities[j]);
                _passiveAbilitiesButtons[j].Clicked -= OnPassiveAbilityButtonClicked;
                _passiveAbilitiesButtons[j].Clicked += OnPassiveAbilityButtonClicked;
            }
        }

        private void OnActiveAbilityButtonClicked(SmallAbilityButton button)
        {
            var descriptionPanel = (AbilityDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.AbilityDescription);
            var stats = _currentBattleCharacterInfo?.Model.Stats ?? _currentCharacterInfo.GetBattleStats();
            descriptionPanel.UpdateAbilityDescription(button.AbilityInfo, stats);
        }

        private void OnPassiveAbilityButtonClicked(SmallAbilityButton button)
        {
            var passiveSkillsPanel = (PassiveSkillDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.PassiveSkillsDescription);
            passiveSkillsPanel.AssignPassiveSkillsDescription(_passiveAbilitiesButtons.Select(b => b.AbilityInfo).ToArray());
        }
    }
}