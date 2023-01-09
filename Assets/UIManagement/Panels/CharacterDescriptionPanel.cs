using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIManagement.Elements;
using System.Linq;
using Sirenix.OdinInspector;
using UIManagement.trashToRemove_Mockups;

namespace UIManagement
{
    public class CharacterDescriptionPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.CharacterDetails;
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

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateCharacterDescription(BattleCharacterView characterView)
        {
            if (characterView == null)
            {
                throw new System.ArgumentException();
            }
            _characterAvatar.sprite = characterView.AvatarFull;
            _characterName.text = characterView.CharacterName;
            var battleStats = characterView.Model.Stats;
            if (_characterStats.Count != 5)
                throw new System.InvalidOperationException();
            _characterStats[0].Text = "��������";
            _characterStats[1].Text = "����";
            _characterStats[2].Text = "�����";
            _characterStats[3].Text = "���������";
            _characterStats[4].Text = "��������";
            _characterStats[0].Value = battleStats.UnmodifiedHealth.ToString();
            _characterStats[1].Value = battleStats.UnmodifiedAttack.ToString();
            _characterStats[2].Value = battleStats.UnmodifiedArmor.ToString();
            _characterStats[3].Value = battleStats.UnmodifiedEvasion.ToString();
            _characterStats[4].Value = battleStats.UnmodifiedAccuracy.ToString();

            var activeAbilities = characterView.ActiveAbilitiesView.Where(v => !_ignoredActiveAbilities.Contains(v.AbilityInfo)).ToArray();
            var passiveAbilities = characterView.PassiveAbilitiesView;
            if (activeAbilities.Length != _activeAbilitiesButtons.Count
                || passiveAbilities.Length != _passiveAbilitiesButtons.Count)
                throw new System.InvalidOperationException();
            for (var i = 0; i < activeAbilities.Length; i++)
            {
                _activeAbilitiesButtons[i].AssignAbilityView(activeAbilities[i]);
                _activeAbilitiesButtons[i].Clicked -= OnActiveAbilityButtonClicked;
                _activeAbilitiesButtons[i].Clicked += OnActiveAbilityButtonClicked;
            }
            for (var j = 0; j < passiveAbilities.Length; j++)
            {
                _passiveAbilitiesButtons[j].AssignAbilityView(passiveAbilities[j]);
                _passiveAbilitiesButtons[j].Clicked -= OnPassiveAbilityButtonClicked;
                _passiveAbilitiesButtons[j].Clicked += OnPassiveAbilityButtonClicked;
            }
        }

        private void OnActiveAbilityButtonClicked(SmallAbilityButton button)
        {
            var descriptionPanel = (AbilityDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.AbilityDescription);
            descriptionPanel.UpdateAbilityDescription(button.AbilityView);
        }

        private void OnPassiveAbilityButtonClicked(SmallAbilityButton button)
        {
            var passiveSkillsPanel = (PassiveSkillDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.PassiveSkillsDescription);
            passiveSkillsPanel.AssignPassiveSkillsDescription(_passiveAbilitiesButtons.Select(b => b.AbilityView).ToArray());
        }
    }
}