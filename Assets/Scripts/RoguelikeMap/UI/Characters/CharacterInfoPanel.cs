using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using Inventory_Items;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.MetaGame;
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
        [SerializeField] 
        private PickItemInventoryPresenter _playerInventoryPresenter;
        
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
        
        public void InitializeCharacterInfo(GameCharacter character)
        {
            _characterName.text = character.CharacterData.Name;
            _characterAvatar.sprite = character.CharacterData.Avatar;
            InitializeStatsText(character.BattleStats);
            //var activeAbilityInfos = character.GetActiveAbilityInfos().Skip(1).ToArray();
            //_passiveAbilityInfos = character.GetPassiveAbilityInfos().ToArray();
            //InitializeAbilityButtons(activeAbilityInfos);
            if (_playerInventoryPresenter is not null)
                _playerInventoryPresenter.UpdateTargetInventory(character.Inventory);
        }

        private void InitializeStatsText(IBattleStats stats)
        {
            _hpText.text = $"{stats[BattleStat.MaxHealth].ModifiedValue}";
            _damageText.text = $"{stats[BattleStat.AttackDamage].ModifiedValue}";
            _armorText.text = $"{stats[BattleStat.MaxArmor].ModifiedValue}";
            _evasionText.text = $"{stats[BattleStat.Evasion].ModifiedValue}";
            _accuracyText.text = $"{stats[BattleStat.Accuracy].ModifiedValue}";
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