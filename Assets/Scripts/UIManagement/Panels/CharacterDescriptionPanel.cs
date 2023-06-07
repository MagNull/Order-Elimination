using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using Sirenix.OdinInspector;
using OrderElimination.AbilitySystem;

namespace UIManagement
{
    public class CharacterDescriptionPanel : UIPanel
    {
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
            UpdateBattleStats(character.BattleStats);

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

        public void UpdateCharacterDescription(OrderElimination.Character characterInfo)
        {
            if (characterInfo == null)
            {
                throw new System.ArgumentException();
            }
            var battleStats = characterInfo.GetBattleStats();
            var activeAbilities = characterInfo.GetActiveAbilityInfos().Where(i => !_ignoredActiveAbilities.Contains(i)).ToArray();
            var passiveAbilities = characterInfo.GetPassiveAbilityInfos();
            _currentCharacterInfo = characterInfo;
            _currentBattleCharacterInfo = null;
            _characterAvatar.sprite = characterInfo.Avatar;
            _characterName.text = characterInfo.Name;
            //UpdateBattleStats(battleStats);
            UpdateAbilityButtonsInfo(activeAbilities, passiveAbilities);
        }

        private void UpdateBattleStats(BattleStats battleStats)
        {
            if (_characterStats.Count != 5)
                throw new System.InvalidOperationException();
            _characterStats[0].Text = "Здоровье";
            _characterStats[1].Text = "Урон";
            _characterStats[2].Text = "Броня";
            _characterStats[3].Text = "Уклонение";
            _characterStats[4].Text = "Точность";
            _characterStats[0].Value = battleStats[BattleStat.MaxHealth].ModifiedValue.ToString();
            _characterStats[1].Value = battleStats[BattleStat.AttackDamage].ModifiedValue.ToString();
            _characterStats[2].Value = battleStats[BattleStat.MaxArmor].ModifiedValue.ToString();
            _characterStats[3].Value = $"{battleStats[BattleStat.Evasion].ModifiedValue * 100}%";
            _characterStats[4].Value = $"{battleStats[BattleStat.Accuracy].ModifiedValue * 100}%";
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