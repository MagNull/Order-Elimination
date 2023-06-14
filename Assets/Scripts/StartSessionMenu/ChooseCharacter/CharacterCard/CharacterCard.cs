using System;
using Cysharp.Threading.Tasks.Triggers;
using OrderElimination;
using OrderElimination.MetaGame;
using UIManagement.Elements;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour
    {
        private bool _isSelected;

        [SerializeField] 
        protected Image _cardImage;
        [SerializeField]
        private HoldableButton _button;
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
            //TODO Remove downcast. Keep changing Character with GameCharacter
            Character = character;
            _cardImage.sprite = character.CharacterData.Avatar;
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
