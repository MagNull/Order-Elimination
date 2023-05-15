using System;
using System.Linq;
using OrderElimination;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace StartSessionMenu.ChooseCharacter.CharacterCard
{
    public class CharacterCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,  IPointerClickHandler
    {
        [SerializeField] 
        protected Image _cardImage;
        
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
        }

        public virtual void Select()
        {
            IsSelected = !IsSelected;
        }

        public void OnBeginDrag(PointerEventData eventData)
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

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick");
            OnGetInfo?.Invoke(this);
        }
    }
}
