using System;
using OrderElimination;
using OrderElimination.MetaGame;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour, IPointerClickHandler
    {
        private bool _isSelected;

        [SerializeField] 
        protected Image _cardImage;
        public bool IsSelected
        {
            get => _isSelected;
            protected set
            {
                _isSelected = value;
                if (value == true)
                    Selected?.Invoke(this);
                else
                    Deselected?.Invoke(this);
            }
        }
        public GameCharacter Character { get; private set; }
        public event Action<CharacterCard> OnGetInfo;
        public event Action<CharacterCard> Selected;
        public event Action<CharacterCard> Deselected;
        
        public virtual void InitializeCard(GameCharacter character, bool isSelected)
        {
            Character = character;
            _cardImage.sprite = character.CharacterData.Avatar;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            Logging.Log("OnClick");
            OnGetInfo?.Invoke(this);
        }
    }
}
