using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using RoguelikeMap;

namespace OrderElimination
{
    public class CharacterCard : MonoBehaviour
    {
        private Character _character;
        private int _cost;

        public Character Character => _character;
        public int Cost => _cost;

        public Button Button => _button;

        private HealthBar _healthBar;
        [SerializeField] 
        private Image _cardImage;
        [SerializeField]
        private Text _cardName;
        [SerializeField]
        private Text _cardCost;
        [SerializeField] 
        private Button _button;

        public bool _isSelected;

        public void InitializeCard(Character character, int cost)
        {
            _character = character;
            _cost = cost;
            _cardImage.sprite = character.GetViewAvatar();
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar?.SetMaxHealth(character.GetBattleStats().UnmodifiedHealth);
            if(_cardName is not null)
                _cardName.text = character.GetName();
            if(_cardCost is not null)
                _cardCost.text = cost.ToString() + "$";
            _button.onClick = new Button.ButtonClickedEvent();
        }

        public void Select()
        {
            _isSelected = !_isSelected;
            _cardCost.gameObject.SetActive(!_isSelected);
        }
    }   
}
