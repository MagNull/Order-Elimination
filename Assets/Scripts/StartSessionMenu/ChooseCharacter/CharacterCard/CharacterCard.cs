using OrderElimination;
using UnityEngine;
using UnityEngine.UI;
using UIManagement;
using UIManagement.Elements;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour
    {
        protected Character _character;
        public bool IsSelected;
        
        [SerializeField] 
        protected Image _cardImage;
        [SerializeField] 
        protected Button _button;
        
        public Character Character => _character;
        public Button Button => _button;
        private HoldableButton _holdableButton;

        public bool _isSelected;

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
        
        private void OnHold(HoldableButton b, float t)
        {
            var charDescPanel =
                (CharacterDescriptionPanel) UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
            charDescPanel.UpdateCharacterDescription(_character);
        }
    }
}
