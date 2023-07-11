using System;
using OrderElimination;
using Sirenix.OdinInspector;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoguelikeMap.UI.Characters
{
    public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private bool _isTransparency;
        [SerializeField, ShowIf("_isTransparency")]
        private Image _image;
        [SerializeField, Range(0, 1), ShowIf("_isTransparency")] 
        private float _aplha;
        [field: SerializeField]
        public bool IsCreateCopy { get; private set; }
        
        private Transform _initialParent;
        private Transform _defaultParent;
        private CanvasGroup _canvasGroup;
        private CharacterCard _dragObject;
        public CharacterCard DragObject
        {
            get => _dragObject;
            private set => _dragObject = value; 
        }

        private void Start()
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }
        
        private CharacterCard InitializeDragObject()
        {
            _initialParent = transform.parent;
            _defaultParent = _initialParent.parent.parent;
            var card = GetComponent<CharacterCard>();
            if (!IsCreateCopy)
                return card;
            var dragObjectCopy = Instantiate(this, transform.position, transform.rotation, _defaultParent);
            var characterCardCopy = dragObjectCopy.GetComponent<CharacterCard>();
            Destroy(dragObjectCopy);
            characterCardCopy.InitializeCard(card.Character, false);
            _canvasGroup = characterCardCopy.GetComponent<CanvasGroup>();
            return characterCardCopy;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            DragObject = InitializeDragObject();
            DragObject.transform.SetParent(_defaultParent);
            if(_isTransparency)
                ChangeTransparency(_aplha);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragObject.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragObject.transform.parent == _defaultParent)
            {
                if (IsCreateCopy)
                    Destroy(DragObject.gameObject);
                else
                {
                    DragObject.transform.SetParent(_initialParent);
                    DragObject.transform.localPosition = Vector3.zero;
                }
            }
            if (_isTransparency)
                ChangeTransparency();
            _canvasGroup.blocksRaycasts = true;
        }

        private void ChangeTransparency(float alpha = 1)
        {
            if(_image is null)
                Logging.LogException(new NullReferenceException("Image is null in DraggableObject"));
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}