using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OrderElimination
{
    public class CharacterCard : MonoBehaviour
    {
        private Character _character;
        private int _cost;

        public Character Character => _character;
        public int Cost => _cost;

        public Button Button => _button;

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
            _cardName.text = character.GetName();
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
