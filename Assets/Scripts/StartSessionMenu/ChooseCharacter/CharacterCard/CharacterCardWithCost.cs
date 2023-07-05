using OrderElimination.MacroGame;
using RoguelikeMap.UI.Characters;
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

        private DropZone _dropZone;

        public override void InitializeCard(GameCharacter character, bool isSelected)
        {
            _cost = character.CharacterData.Price;
            base.InitializeCard(character, isSelected);
            _cardCost.text = _cost + "$";
        }

        public void SetDropZone(DropZone dropZone)
        {
            if(_dropZone is not null)
                _dropZone.Select();
            _dropZone = dropZone;
            _dropZone.Select();
        }
        
        public override void Select()
        {
            _costImage.gameObject.SetActive(IsSelected);
            base.Select();
        }
    }
}
