using System;
using System.Linq;
using OrderElimination;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] 
        protected Image _cardImage;
        [SerializeField] 
        private HoldableButton _button;
        
        protected Transform _initialParent;
        
        protected Character _character;
        protected Transform _defaultParent;
        
        public bool IsSelected { get; private set; }
        public Character Character => _character;
        public event Action<CharacterCard> OnTrySelect;
        public event Action<CharacterCard> OnUnselect;
        public event Action<CharacterCard> OnGetInfo;
        
        public virtual void InitializeCard(Character character, Transform defaultParent)
        {
            _character = character;
            _cardImage.sprite = character.Avatar;
            _defaultParent = defaultParent;
            _button.Clicked += OnClick;
            _button.Holded += OnBeginDrag;
        }

        public virtual void Select()
        {
            IsSelected = !IsSelected;
        }

        private void OnBeginDrag(HoldableButton button, float holdTimeInSecond)
        {
            Debug.Log("OnBeginDrag");
            _initialParent = transform.parent;
            transform.SetParent(_defaultParent);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
            if (!IsSelected)
            {
                if (eventData.hovered.Any(x => x.CompareTag("SelectedDropZone")))
                    OnTrySelect?.Invoke(this);
                else
                    transform.SetParent(_initialParent);
            }
            else
            {
                if (eventData.hovered.Any(x => x.CompareTag("UnselectedDropZone")))
                    OnTrySelect?.Invoke(this);
                else
                    transform.SetParent(_initialParent);
            }
        }

        private void OnClick(HoldableButton button)
        {
            Debug.Log("OnClick");
            OnGetInfo?.Invoke(this);
        }
    }
}
