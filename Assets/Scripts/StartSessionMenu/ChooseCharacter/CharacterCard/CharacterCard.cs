using OrderElimination;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour
    {
        protected Character _character;
        [FormerlySerializedAs("_isSelected")] 
        public bool IsSelected;
        
        [SerializeField] 
        protected Image _cardImage;
        [SerializeField] 
        protected Button _button;
        
        public Character Character => _character;
        public Button Button => _button;

        public virtual void InitializeCard(Character character)
        {
            _character = character;
            _cardImage.sprite = character.GetViewAvatar();
            _button.onClick = new Button.ButtonClickedEvent();
        }

        public virtual void Select()
        {
            IsSelected = !IsSelected;
        }
    }
}