using OrderElimination;
using UnityEngine;
using UnityEngine.UI;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCardWithCost : CharacterCard
    {
        private int _cost = 400;
        public int Cost => _cost;
        
        [SerializeField]
        private Image _costImage;
        [SerializeField]
        private Text _cardCost;

        public override void InitializeCard(Character character)
        {
            _character = character;
            _cardImage.sprite = character.GetViewAvatar();
            _cardCost.text = _cost + "$";
            _button.onClick = new Button.ButtonClickedEvent();
        }

        public override void Select()
        {
            _costImage.gameObject.SetActive(IsSelected);
            base.Select();
        }
    }
}
