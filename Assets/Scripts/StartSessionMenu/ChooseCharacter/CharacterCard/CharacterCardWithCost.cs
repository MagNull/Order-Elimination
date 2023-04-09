using OrderElimination;
using UnityEngine;
using UnityEngine.UI;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardWithCost : ICharacterCard
    {
        private int _cost;
        public int Cost => _cost;
        
        [SerializeField]
        private Image _costImage;
        [SerializeField]
        private Text _cardCost;

        public void InitializeCard(Character character, int cost)
        {
            _character = character;
            _cost = cost;
            _cardImage.sprite = character.GetViewAvatar();
            _cardCost.text = cost + "$";
            _button.onClick = new Button.ButtonClickedEvent();
        }

        public override void Select()
        {
            _costImage.gameObject.SetActive(!_isSelected);
            base.Select();
        }
    }
}
