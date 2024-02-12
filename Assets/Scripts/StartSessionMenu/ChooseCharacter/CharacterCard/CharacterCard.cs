using System;
using OrderElimination;
using OrderElimination.MacroGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour, IPointerClickHandler
    {
        private bool _isSelected;

        [SerializeField]
        protected Image _cardImage;

        [SerializeField]
        private Image _highlightBorder;

        public bool IsSelected
        {
            get => _isSelected;
            private set => _isSelected = value;
        }

        public GameCharacter Character { get; private set; }

        private UnityEvent _specialClickEvent;
        public event Action<CharacterCard> OnClicked;

        //TODO: Replace with CharacterTemplate or make separate initialization
        public virtual void InitializeCard(GameCharacter character, bool isSelected)
        {
            Character = character;
            _cardImage.sprite = character.CharacterData.Avatar;
            UpdateColor();
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

        public void UpdateColor()
        {
            _cardImage.color = Character.CurrentHealth <= 0 ? Color.red : Color.white;
        }

        public void SetSpecialClickEvent(UnityAction action)
        {
            if (action == null)
            {
                Logging.LogException(new ArgumentException("Try set null special event"), this);
                throw new();
            }
            
            _specialClickEvent = new UnityEvent();
            _specialClickEvent.AddListener(action);
        }

        public void EnableHighlight() => _highlightBorder.enabled = true;
        public void DisableHighlight() => _highlightBorder.enabled = false;

        public void ResetSpecialClickEvent()
        {
            if(_specialClickEvent == null)
                return;
            
            _specialClickEvent.RemoveAllListeners();
            _specialClickEvent = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_specialClickEvent != null)
            {
                _specialClickEvent.Invoke();
                _specialClickEvent = null;
            }
            else
                OnClicked?.Invoke(this);
        }
    }
}