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

        public override void InitializeCard(Character character, bool isSelected)
        {
            base.InitializeCard(character, isSelected);
            _cardCost.text = _cost + "$";
        }

        public override void Select()
        {
            _costImage.gameObject.SetActive(IsSelected);
            base.Select();
        }
    }
}
