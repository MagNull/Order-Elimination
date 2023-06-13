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
        [SerializeField]
        private List<AbilityButton> _activeAbilityButtons = new ();
        [SerializeField]
        private List<PassiveAbilityButton> _passiveAbilityButtons = new();

        private AbilityInfo[] _passiveAbilityInfos;
        
        public void InitializeCharacterInfo(GameCharacter character)
        {
            if(_playerInventoryPresenter is not null)
                _characterInventoryPresenter.InitInventoryModel(character.Inventory);
            InitializeStatsText(
                character.CharacterStats.MaxHealth,
                character.CharacterStats.MaxArmor,
                character.CharacterStats.AttackDamage,
                character.CharacterStats.Accuracy,
                character.CharacterStats.Evasion);
            _characterAvatar.sprite = character.CharacterData.Avatar;
            _characterName.text = character.CharacterData.Name;
            _playerInventoryPresenter.UpdateTargetInventory(character.Inventory);
        }

        private void InitializeStatsText(
            float maxHealth, float maxArmor, float attack, float accuracy, float evasion)
        {
            _hpText.text = maxHealth.ToString();
            _armorText.text = maxArmor.ToString();
            _damageText.text = attack.ToString();
            _accuracyText.text = accuracy.ToString();
            _evasionText.text = evasion.ToString();
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