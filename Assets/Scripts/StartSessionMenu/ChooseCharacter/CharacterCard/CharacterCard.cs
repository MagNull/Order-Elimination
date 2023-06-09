using System;
using OrderElimination;
using UIManagement.Elements;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour
    {
        [SerializeField] 
        protected Image _cardImage;
        [SerializeField]
        private HoldableButton _button;
        
        private Character _character;
        private Transform _defaultParent;
        public bool IsSelected { get; protected set; }
        public Character Character => _character;
        public event Action<CharacterCard> OnGetInfo;
        
        public virtual void InitializeCard(Character character, bool isSelected)
        {
            _character = character;
            _cardImage.sprite = character.Avatar;
            _button.Clicked += OnClick;
            IsSelected = isSelected;
        }

        public virtual void Select()
        {
            IsSelected = !IsSelected;
        }
        
        public void SetImage(Sprite sprite)
        {
            _cardImage.sprite = sprite;
        }

        private void OnClick(HoldableButton button)
        {
            Debug.Log("OnClick");
            OnGetInfo?.Invoke(this);
        }
    }
}
