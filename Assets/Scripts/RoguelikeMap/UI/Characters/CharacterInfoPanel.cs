using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using OrderElimination;
using RoguelikeMap.UI.Abilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI.Characters
{
    public class CharacterInfoPanel : Panel
    {
        [SerializeField]
        private AbilityInfoPanel _abilityInfoPanel;
        [SerializeField] 
        private PassiveAbilityInfoPanel _passiveAbilityInfoPanel;
        
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
        [SerializeField]
        private List<AbilityButton> _activeAbilityButtons = new ();
        [SerializeField]
        private List<PassiveAbilityButton> _passiveAbilityButtons = new();

        private AbilityInfo[] _passiveAbilityInfos;
        
        public void InitializeCharacterInfo(Character character)
        {
            _characterName.text = character.name;
            _characterAvatar.sprite = character.Avatar;
            var stats = character.GetBattleStats();
            InitializeStatsText(stats);
            
            var activeAbilityInfos = character.GetActiveAbilityInfos().Skip(1).ToArray();
            _passiveAbilityInfos = character.GetPassiveAbilityInfos().ToArray();
            InitializeAbilityButtons(activeAbilityInfos);
        }

        private void InitializeStatsText(IReadOnlyBattleStats stats)
        {
            _hpText.text = $"{stats.Health}";
            _damageText.text = $"{stats.Attack}";
            _armorText.text = $"{stats.Armor}";
            _evasionText.text = $"{stats.Evasion}";
            _accuracyText.text = $"{stats.Accuracy}";
        }

        private void InitializeAbilityButtons(AbilityInfo[] activeAbilityInfos)
        {
            for (var i = 0; i < activeAbilityInfos.Length; i++)
            {
                _activeAbilityButtons[i].SetAbilityInfo(activeAbilityInfos[i]);
                _activeAbilityButtons[i].OnClick += OnAbilityClick;
            }
            
            for (var i = 0; i < _passiveAbilityButtons.Count; i++)
            {
                _passiveAbilityButtons[i].SetAbilityInfos(_passiveAbilityInfos[i]);
                _passiveAbilityButtons[i].OnClick += OnPassiveAbilityClick;
            }
        }

        private void OnAbilityClick(AbilityInfo abilityInfo)
        {
            Debug.Log("OnAbilityClicked");
            _abilityInfoPanel.InitializeInfo(abilityInfo);
            _abilityInfoPanel.Open();
        }

        private void OnPassiveAbilityClick()
        {
            _passiveAbilityInfoPanel.InitializeInfo(_passiveAbilityInfos);
            _passiveAbilityInfoPanel.Open();
        }
    }
}