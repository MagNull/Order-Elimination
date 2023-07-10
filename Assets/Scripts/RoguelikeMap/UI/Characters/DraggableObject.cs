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
        private Transform _dragTransform;
        private DraggableObject _dragObject;
        public bool IsCopy { get; private set; } = false;
        
        public DraggableObject DragObject
        {
            get => IsCreateCopy ? _dragObject : null;
            private set => _dragObject = value; 
        }

        private void Start()
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }
        
        private Transform InitializeTransform()
        {
            _initialParent = transform.parent;
            _defaultParent = _initialParent.parent.parent;
            if (!IsCreateCopy || IsCopy)
                return transform;
            DragObject = Instantiate(this, transform.position, transform.rotation, _defaultParent);
            DragObject.IsCopy = true;
            var copyCard = DragObject.GetComponent<CharacterCard>();
            copyCard.InitializeCard(GetComponent<CharacterCard>().Character, false);
            _canvasGroup = DragObject.GetComponent<CanvasGroup>();
            return DragObject.transform;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragTransform = InitializeTransform();
            _dragTransform.SetParent(_defaultParent);
            if(_isTransparency)
                ChangeTransparency(_aplha);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragTransform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_dragTransform.parent == _defaultParent)
            {
                if(IsCreateCopy && IsCopy)
                    Destroy(gameObject);
                else if (IsCreateCopy)
                    Destroy(DragObject.gameObject);
                else
                {
                    _dragTransform.SetParent(_initialParent);
                    _dragTransform.localPosition = Vector3.zero;    
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