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
        private IconTextValueList _characterStats;
        [SerializeReference]
        private List<SmallSkillButton> _activeAbilitiesButtons;
        [SerializeReference]
        private List<SmallSkillButton> _passiveAbilitiesButtons;
        [SerializeReference]
        private List<AbilityInfo> _ignoredActiveAbilities;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void UpdateCharacterDescription(BattleCharacterView characterView)
        {
            _characterAvatar.sprite = characterView.AvatarFull;
            _characterName.text = characterView.name;
            var battleStats = characterView.Model.Stats;
            // Stats


            var activeAbilities = characterView.ActiveAbilitiesView.Where(v => !_ignoredActiveAbilities.Contains(v.AbilityInfo)).ToArray();
            var passiveAbilities = characterView.PassiveAbilitiesView;
            if (activeAbilities.Length != _activeAbilitiesButtons.Count
                || passiveAbilities.Length != _passiveAbilitiesButtons.Count)
                throw new System.InvalidOperationException();
            for (var i = 0; i < activeAbilities.Length; i++)
            {
                _activeAbilitiesButtons[i].AssignAbilityView(activeAbilities[i]);
            }
            for (var j = 0; j < passiveAbilities.Length; j++)
            {
                _passiveAbilitiesButtons[j].AssignAbilityView(passiveAbilities[j]);
            }
        }
    }
}